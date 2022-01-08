using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ecl.Unicode.Ucd {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// http://www.unicode.org/reports/tr44/
    /// </remarks>
    public partial class UcdLoader : IDisposable {
        private UnicodeEntry[] _entries;
        private int[] _decomposings;

        private LoadOptions _options;
        private ZipLoader _fileLoader;
        private readonly int _maxCodePoint;

        public UcdLoader( string ucdFileName, LoadOptions options = 0 ) {
            _fileLoader = new ZipLoader( ucdFileName );
            _options = options;
            if ( ( options & LoadOptions.AllCodes ) != 0 ) {
                _maxCodePoint = 0x10FFFF;
            } else if ( ( options & LoadOptions.EuropeOnly ) != 0 ) {
                _maxCodePoint = 0x052F;
            } else {
                _maxCodePoint= 0xFFFF;
            }
        }

        internal LineReader OpenLineReader( string folder, string name, TextReaderOptions options = 0 ) {
            Stream s = _fileLoader.OpenFile( folder, name );
            if( s != null ) {
                return new LineReader( s, options );
            }
            Error( "Unable to find file '{0}'", name );
            return null;
        }

        internal LineReader OpenLineReader( string name, TextReaderOptions options = 0 ) {
            Stream s = _fileLoader.OpenFile( name );
            if( s != null ) {
                return new LineReader( s, options );
            }
            Error( "Unable to find file '{0}'", name );
            return null;
        }
        internal TextReader OpenText( string name ) {
            var s = _fileLoader.OpenFile( name );
            if ( s != null ) {
                return new StreamReader( s, Encoding.UTF8 );
            }
            return null;
        }
        
        private int IndexOf( int value ) {
            return _entries.BinaryFind( value );
        }

        public bool TryGetEntry( int code, out UnicodeEntry entry ) {
            int idx = IndexOf( code );
            if ( idx >= 0 ) {
                entry = _entries[ idx ];
                return true;
            }
            entry = default(UnicodeEntry);
            return false;
        }

        public UnicodeEntry this[ int code ] {
            get {
                int idx = IndexOf( code );
                if ( idx >= 0 ) {
                    return _entries[ idx ];
                }
                return default(UnicodeEntry);
            }
        }

        public bool AddDecomposing( int code, List<int> list, bool recurse = false ) {
            int idx = IndexOf( code );
            if( idx >= 0 ) {
                int a= _entries[ idx ]._decomposingStart;
                if ( a != 0 ) {
                    int b = _entries[ idx ]._decomposingStart2;
                    if ( a > 0 ) {
                        if ( !recurse || !AddDecomposing( a, list, true ) ) {
                            list.Add( a );
                        }
                        if ( b != 0 ) {
                            if( !recurse || !AddDecomposing( b, list, true ) ) {
                                list.Add( b );
                            }
                        }
                    } else {
                        a = -a;
                        while ( true ) {
                            code = _decomposings[ b ];
                            if( !recurse || !AddDecomposing( code, list, true ) ) {
                                list.Add( code );
                            }
                            a--;
                            if ( a == 0 )
                                break;
                            b++;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        private UcdBlock[] _blocks;
        /// <summary>
        /// 
        /// </summary>
        public UcdBlock[] Blocks {
            get {
                if ( _blocks == null ) {
                    _blocks = LoadBlocks();
                }
                return _blocks;
            }
        }
        public EnumRange<EmojiType>[] LoadEmoji() {
            var list = new List<EnumRange<EmojiType>>();
            using ( LineReader reader = OpenLineReader( "emoji", "emoji-data.txt" ) ) {
                foreach ( var range in GetRanges( reader, Util.ParseEmoji ) ) {
                    list.Add( range );
                }
            }
            return list.ToArray();
        }
        public EnumRange<WordBreak>[] LoadWordBreak() {
            var list = new List<EnumRange<WordBreak>>();
            using( LineReader reader = OpenLineReader( "auxiliary", "WordBreakProperty.txt" ) ) {
                foreach( var range in GetRanges( reader, Util.ParseWordBreak ) ) {
                    list.Add( range );
                }
            }
            return list.ToArray();
        }
        public EnumRange<SentenceBreak>[] LoadSentenceBreak() {
            var list = new List<EnumRange<SentenceBreak>>();
            using( LineReader reader = OpenLineReader( "auxiliary", "SentenceBreakProperty.txt" ) ) {
                foreach( var range in GetRanges( reader, Util.ParseSentenceBreak ) ) {
                    list.Add( range );
                }
            }
            return list.ToArray();
        }

        public EnumRange<GraphemeClusterBreak>[] LoadGraphemeBreak() {
            var list = new List<EnumRange<GraphemeClusterBreak>>();
            using( LineReader reader = OpenLineReader( "auxiliary", "GraphemeBreakProperty.txt" ) ) {
                foreach( var range in GetRanges( reader, Util.ParseGraphemeClusterBreak ) ) {
                    list.Add( range );
                }
            }
            return list.ToArray();
        }

        private UcdBlock[] LoadBlocks() {
            List<UcdBlock> list = new List<UcdBlock>();
            using( LineReader reader = OpenLineReader( "Blocks.txt" ) ) {
                foreach( NamedRange range in GetNamedRanges( reader ) ) {
                    list.Add( new UcdBlock( range.Begin, range.End, range.Name ) );
                }
            }
            var blocks = list.ToArray();
            UcdRange.Sort( blocks );
            return blocks;
        }
        private UcdRange<UcdCodeProperty>[] _propertyRanges;

        private UcdCodeProperty[] _allProperties;

        public UcdCodeProperty[] CodeProperties {
            get {
                if( _allProperties == null ) {
                    LoadProperties();
                }
                return _allProperties;
            }
        }

        public UcdCodeProperty GetCodeProperty( CodePointProperty property ) {
            foreach ( UcdCodeProperty p in CodeProperties ) {
                if ( p.Value == property ) {
                    return p;
                }
            }

            return null;
        }
        public UcdCodeProperty[] GetCodeProperties( int codePoint ) {
            if( CodeProperties.Length > 0 ) {
                int end;
                int i = XUtil.GetRange( _propertyRanges, codePoint, out end );

                if ( i >= 0 ) {
                    List<UcdCodeProperty> list = new List<UcdCodeProperty>();

                    for ( ; i < end; i++ ) {
                        var prop = _propertyRanges[ i ].Owner;
                        if ( list.IndexOf( prop ) < 0 ) {
                            list.Add( prop );
                        }
                    }
                    return list.ToArray();
                }
            }
            return null;
        }
        private void LoadProperties() {
            var all = new List<UcdRange<UcdCodeProperty>>();
            var map = new Dictionary<string, UcdCodeProperty>( StringComparer.OrdinalIgnoreCase );
            var list = new List<UcdCodeProperty>();
            var _codePropMap = Util.GetPropertyMap();
            CodePointProperty _lastValue = (CodePointProperty)(Enum.GetValues( typeof( CodePointProperty ) ).Length + 1);
            foreach ( var value in Enum.GetValues( typeof( CodePointProperty ) ) ) {
                Debug.WriteLine( value.ToString() );
            }
            void ReadFile(string fileName,bool isDerived) {
                using ( LineReader reader = OpenLineReader( fileName ) ) {
                    foreach ( NamedRange range in GetNamedRanges( reader ) ) {
                        UcdCodeProperty prop;
                        if ( !map.TryGetValue( range.Name, out prop ) ) {
                            CodePointProperty propVal;
                            if ( !_codePropMap.TryGetValue( range.Name, out propVal ) ) {
                                propVal = _lastValue++;
                                _codePropMap.Add( range.Name, propVal );
                                //Debug.WriteLine( $"New Property {range.Name}:{(int)propVal} ({isDerived})" );
                            }
                            //var val = ParseCodeProperty( range.Name );
                            prop = new UcdCodeProperty( range.Name, propVal ) {
                                IsDerived = isDerived
                            };
                            map.Add( range.Name, prop );
                            list.Add( prop );
                        }
                        var propRange = new UcdRange<UcdCodeProperty>( range.Begin, range.End, prop );
                        prop.Ranges.Add( propRange );
                        all.Add( propRange );
                    }
                }
            }

            ReadFile( "PropList.txt", false );
            ReadFile( "DerivedCoreProperties.txt", true );
            _allProperties = list.ToArray();
            _propertyRanges = all.ToArray();
            UcdRange.Sort( _propertyRanges );
        }

        public IEnumerable<UnicodeEntry> GetCodePoints( int begin, int end ) {
            EnsureDataLoaded();
            if ( begin <= end ) {
                int idx = IndexOf( begin );
                if ( idx < 0 ) {
                    idx = ~idx;
                }

                for ( ; idx < _entries.Length &&
                    _entries[ idx ].CodeValue <= end; idx++ ) {
                    yield return _entries[ idx ];
                }
            }
        }

        public IEnumerable<UnicodeEntry> GetCodePoints( UcdRange range ) {
            return GetCodePoints( range.Begin, range.End );
        }
        public IEnumerable<UnicodeEntry> GetCodePoints( IEnumerable<int> codes ) {
            EnsureDataLoaded();
            foreach ( int code in codes ) {
                UnicodeEntry entry;
                if ( TryGetEntry( code, out entry ) ) {
                    yield return entry;
                }
            }
        }
        public IEnumerable<UnicodeEntry> GetCodePoints() {
            EnsureDataLoaded();
            return _entries;
        }
        public void Dispose() {
            _fileLoader.Dispose();
        }

        public UcdBlock GetBlock( int codeValue ) {
            if ( Blocks != null ) {
                int idx = _blocks.BinaryFind( codeValue );
                if ( idx >= 0 ) {
                    return _blocks[ idx ];
                }
            }
            return null;
        }
    }
}
