using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ecl.Unicode;

namespace eclUnicode.Ucd {
    // http://www.unicode.org/reports/tr44/#UnicodeData.txt
    partial class UcdLoader {
        private static bool TryParseHex( string text, out int code ) {
            return int.TryParse( text, NumberStyles.HexNumber,
                CultureInfo.InvariantCulture, out code );
        }
        protected virtual void Error( string format, params object[] args ) {
            throw new InvalidDataException( string.Format( format, args ) );
        }

        private Dictionary<int, string> _alternativeNames;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, string> AlternativeNames {
            get {
                if( _alternativeNames == null ) {
                    _alternativeNames = Util.GetAlternativeNames();
                }
                return _alternativeNames;
            }
        }

        [Flags]
        public enum LoadOptions {
            /// <summary>
            /// Load entries above 0xFFFF
            /// </summary>
            AllCodes = 1,

            /// <summary>
            /// don't load decompose information
            /// </summary>
            IgnoreDecombosing = 2,

            /// <summary>
            /// Load old names
            /// </summary>
            OldNames = 4,

            /// <summary>
            /// Load old comments
            /// </summary>
            Comments = 8,

            EnumNames = 16,

            AlternativeNames = 32,
            Script = 64,
            ScriptExtensions = 128,
            /// <summary>
            /// Don'Load entries above 0x052F
            /// </summary>
            EuropeOnly = 256
        };
        private static readonly char[] _spaceDelims = { ' ' };

        private void LoadData() {
            Loader ldr = new Loader( this );
            using (LineReader reader = OpenLineReader ( "UnicodeData.txt" ) ) {
                ldr.Load( reader );
            }
            _entries = ldr.GetEntries();
            _decomposings = ldr._decomposings.ToArray();
        }
        public void EnsureDataLoaded() {
            if( _entries == null ) {
                LoadData();
            }
        }
        public class Loader {
            public readonly LoadOptions Options;

            private const int WorkSegment = 1024;
            private readonly List<UnicodeEntry[]> _segList = new List<UnicodeEntry[]>( 16 );
            private UnicodeEntry[] _workEntries = new UnicodeEntry[ WorkSegment ];
            private int _workIndex;
            private int _totalEntries;
            private UnicodeEntry _entry;
            private readonly Dictionary<string, UnicodeCharacterType> _charTypes = Util.GetTypeMap();
            private readonly Dictionary<string, BidirectionalCategory> _bidiMap = Util.GetBidiMap();
            private readonly Dictionary<string, DecomposingTag> _decomposeMap;
            private List<int> _codeList = new List<int>();
            private UcdLoader _owner;

            public Loader( UcdLoader data ) {
                _owner = data;
                Options = data._options;
                if ( ( Options & LoadOptions.IgnoreDecombosing ) == 0 ) {
                    _decomposeMap = Util.GetDecombosingMap();
                }
            }

            protected void Add( UnicodeEntry entry ) {
                if ( _workIndex == WorkSegment ) {
                    _segList.Add( _workEntries );
                    _workEntries = new UnicodeEntry[ WorkSegment ];
                    _workIndex = 0;
                }
                _workEntries[ _workIndex ] = entry;
                _workIndex++;
                _totalEntries++;
            }

            private void Error( string format, params object[] args ) {
                _owner.Error( format, args );
            }

            public UnicodeEntry[] GetEntries() {
                var allEntries = new UnicodeEntry[ _totalEntries ];
                for ( int i = 0; i < _segList.Count; i++ ) {
                    Array.Copy( _segList[ i ], 0, allEntries, i * WorkSegment, WorkSegment );
                }
                Array.Copy( _workEntries, 0, allEntries, _segList.Count * WorkSegment, _workIndex );
                return allEntries;
            }

            

            

            
            public void Load( LineReader reader ) {
                Dictionary<int, string> alternativeNames = _owner.AlternativeNames;
                List<string> segs = new List<string>();
                foreach ( var count in reader.GetLines(segs,15) ) {
                    if( count != 15 ) {
                        continue;
                    }

                    _entry = new UnicodeEntry();
                    if ( !TryParseHex( segs[ 0 ], out _entry.CodeValue ) ) {
                        continue;
                    }
                    if ( _entry.CodeValue > _owner._maxCodePoint ) {
                        break;
                    }
                    string name = segs[ 1 ];
                    string dscr;
                    if( ( Options & LoadOptions.AlternativeNames ) != 0
                        && alternativeNames.TryGetValue( _entry.CodeValue, out dscr ) ) {
                        name = dscr;
                    } else {
                        dscr = name;
                        if( dscr.SameName( "<control>" ) ) {
                            dscr = segs[ 10 ];
                        }
                    }
                    
                    if( dscr.HasValue() ) {
                        if ( ( Options & LoadOptions.EnumNames ) != 0 ) {
                            _entry.EnumName = Util.GenEnumName( dscr );
                        }
                    } else if ( !alternativeNames.TryGetValue( _entry.CodeValue, out dscr ) ) {
                        Debug.WriteLine( "NO NAME " + _entry.CodeValue.ToString( "X4" ) + " " + segs[ 11 ] );
                    }
                    _entry.Name = name;
                    if ( !_charTypes.TryGetValue( segs[ 2 ], out _entry.Category ) ) {
                        Error( "Unknown category '{0}'", segs[ 2 ] );
                    }
                    byte comb;
                    if ( !byte.TryParse( segs[ 3 ], NumberStyles.Integer, CultureInfo.InvariantCulture, out comb ) ) {
                        Error( "Unknown combing class '{0}'", segs[ 3 ] );
                    }
                    _entry.Combing = (CombingClass)comb;

                    if ( !_bidiMap.TryGetValue( segs[ 4 ], out _entry.Bidirectional ) ) {
                        Error( "Unknown bidi category '{0}'", segs[ 4 ] );
                    }
                    if ( _decomposeMap != null && !string.IsNullOrEmpty( segs[ 5 ] ) ) {
                        ParseDecombosing( segs[ 5 ] );
                    }
                    //_entry._numericType = NumericEntryType.None;
                    byte dec;

                    if ( segs[ 6 ].Length > 0 ) {
                        if ( !byte.TryParse( segs[ 6 ], NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out dec )
                             || dec > 9 ) {
                            Error( "Invalid Decimal digit value '{0}'", segs[ 6 ] );
                        }
                        _entry._numericType = NumericEntryType.Decimal;
                        _entry._value.Value = dec;
                        //_Decimals.Increment( _entry.DecimalValue );
                    }
                    if ( segs[ 7 ].Length > 0 ) {
                        if ( !byte.TryParse( segs[ 7 ], NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out dec )
                             || dec > 9 ) {
                            Error( "Invalid Digit value '{0}'", segs[ 7 ] );
                        }
                        if ( !_entry.IsDecimalDigit ) {
                            _entry._numericType = NumericEntryType.Digit;
                            _entry._value.Value = dec;
                        } else if ( _entry._value.Value != dec ) {
                            Error( "Invalid Digit value '{0}'", segs[ 7 ] );
                        }
                    } else if ( _entry._numericType != NumericEntryType.None ) {
                        Error( "Invalid Digit value '{0}'", segs[ 7 ] );
                    }
                    if ( segs[ 8 ].Length > 0 ) {
                        ParseNumeric( segs[ 8 ] );
                    }
                    switch ( segs[ 9 ] ) {
                    case "":
                    case "N":
                        break;
                    case "Y":
                        _entry.Mirrored = true;
                        break;
                    default:
                        Error( "Invalid Mirrored value '{0}'", segs[ 9 ] );
                        break;
                    }
                    if( ( Options & LoadOptions.OldNames ) != 0
                         && segs[ 10 ].Length > 0 ) {
                        _entry.OldName = segs[ 10 ];
                    }
                    if( ( Options & LoadOptions.Comments ) != 0
                         && segs[ 11 ].Length > 0 ) {
                        _entry.Comment = segs[ 11 ];
                    }
                    if ( segs[ 12 ].Length > 0 ) {
                        if ( !TryParseHex( segs[ 12 ], out _entry.Uppercase ) ) {
                            Error( "Invlaid code point '{0}'", segs[ 12 ] );
                        }
                    }
                    if ( segs[ 13 ].Length > 0 ) {
                        if ( !TryParseHex( segs[ 13 ], out _entry.LowerCase ) ) {
                            Error( "Invlaid code point '{0}'", segs[ 13 ] );
                        }
                    }
                    if ( segs[ 14 ].Length > 0 ) {
                        if ( !TryParseHex( segs[ 14 ], out _entry.TitleCase ) ) {
                            Error( "Invlaid code point '{0}'", segs[ 14 ] );
                        }
                    }
                    Add( _entry );
                }
                //ShowCounts();
            }

            //private Dictionary<int, int> _Decimals = new Dictionary<int, int>();
            //private Dictionary<int, int> _Digits = new Dictionary<int, int>();
            internal List<int> _decomposings = new List<int>();

            private int AddDecomposingCodes() {
                int idx = -1;
                if ( 1 == 1 ) {
                    idx = _decomposings.IndexOf( _codeList );
                }
                if ( idx < 0 ) {
                    idx = _decomposings.Count;
                    _decomposings.AddRange( _codeList );
                } else {
                    _Decomposings.Increment( _codeList.Count );
                }
                return idx;
            }

            private Dictionary<long, int> _standAloneDigits = new Dictionary<long, int>();
            private Dictionary<int, int> _Numerators = new Dictionary<int, int>();
            private Dictionary<int, int> _Denominators = new Dictionary<int, int>();
            private Dictionary<int, int> _Decomposings = new Dictionary<int, int>();

            private void ShowCounts() {
                Debug.WriteLine( "Numeric Unicode points statistics" );
                Debug.WriteLine( "StandAlone" );
                var allKeys = _standAloneDigits.Keys.ToArray();
                Array.Sort( allKeys );
                foreach ( var digit in allKeys ) {
                    Debug.WriteLine( $"{digit} = {_standAloneDigits[ digit ]}" );
                }

                var keys = _Numerators.Keys.ToArray();
                Array.Sort( keys );
                Debug.WriteLine( "_Numerators" );
                foreach ( var digit in keys ) {
                    Debug.WriteLine( $"{digit} = {_Numerators[ digit ]}" );
                }

                keys = _Denominators.Keys.ToArray();
                Array.Sort( keys );
                Debug.WriteLine( "_Denominators" );
                foreach ( var digit in keys ) {
                    Debug.WriteLine( $"{digit} = {_Denominators[ digit ]}" );
                }

                keys = _Decomposings.Keys.ToArray();
                Array.Sort( keys );
                Debug.WriteLine( "_Decomposings" );
                foreach ( var digit in keys ) {
                    Debug.WriteLine( $"{digit} = {_Decomposings[ digit ]}" );
                }
                //keys = _Digits.Keys.ToArray();
                //Array.Sort( keys );
                //Debug.WriteLine( "_Digits" );
                //foreach( var digit in keys ) {
                //    Debug.WriteLine( $"{digit} = {_Digits[ digit ]}" );
                //}
                Debug.WriteLine( "Decomposings : " + _decomposings.Count );

            }

            private void ParseNumeric( string text ) {
                int idx = text.IndexOf( '/' );
                long stl;

                if ( idx > 0 ) {
                    if ( _entry._numericType != NumericEntryType.None ) {
                        Error( "Invalid Numeric '{0}'", text );
                        return;
                    }
                    _entry._numericType = NumericEntryType.Fraction;
                    string sNumerator = text.Substring( 0, idx );
                    string sDenominator = text.Substring( idx + 1 );
                    short numerator;
                    if ( short.TryParse( sNumerator, NumberStyles.Integer, CultureInfo.InvariantCulture, out numerator ) ) {
                        short denominator;
                        if ( short.TryParse( sDenominator, NumberStyles.Integer, CultureInfo.InvariantCulture,
                            out denominator ) ) {
                            _Numerators.Increment( numerator );
                            _Denominators.Increment( denominator );
                            _entry._value.Numerator = numerator;
                            _entry._value.Denominator = denominator;

                            //Debug.WriteLine( $"{numerator}/{denominator}" );
                            return;
                        }
                    }
                } else if ( long.TryParse( text, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out stl ) ) {
                    _standAloneDigits.Increment( stl );
                    if ( _entry._numericType != NumericEntryType.None ) {
                        if ( _entry._value.Value != stl ) {
                            Error( "Invalid Numeric '{0}'", text );
                        }
                        return;
                    }
                    if ( stl >= 1000000000 ) {
                        long md = stl / 1000000000;
                        if ( md * 1000000000 != stl || md > int.MaxValue ) {
                            Error( "Invalid Numeric '{0}'", text );
                            return;
                        }
                        _entry._numericType = NumericEntryType.Billions;
                        _entry._value.Value = (int)md;
                        return;
                    }
                    _entry._numericType = NumericEntryType.Value;
                    _entry._value.Value = (int)stl;

                    //Debug.WriteLine( $"{stl}/1" );
                    return;
                }
                Error( "Invalid Numeric '{0}'", text );
            }

            private Dictionary<string, int> _nameCodeMap = new Dictionary<string, int>( StringComparer.Ordinal );

            private void ParseName( string text ) {
                var segs = text.Split( _spaceDelims, StringSplitOptions.RemoveEmptyEntries );
                _codeList.Clear();
                foreach( var seg in segs ) {
                    int idx;
                    if ( !_nameCodeMap.TryGetValue( seg, out idx ) ) {
                        idx = _nameCodeMap.Count;
                        _nameCodeMap.Add( seg, idx );
                    }
                    _codeList.Add( idx );
                }
            }

            private void ParseDecombosing( string decompose ) {
                var segs = decompose.Split( _spaceDelims );
                _codeList.Clear();
                for ( int i = 0; i < segs.Length; i++ ) {
                    var seg = segs[ i ];
                    if ( seg.Length > 0 ) {
                        if ( seg[ 0 ] == '<' ) {
                            if ( i != 0 ) {
                                Error( "Invalid category '{0}'", segs[ 2 ] );
                            }
                            DecomposingTag tag;
                            if ( !_decomposeMap.TryGetValue( seg, out tag ) ) {
                                Error( "Unknown category '{0}'", segs[ 2 ] );
                            }
                            _entry.Decomposing = tag;
                            //_codeList.Add( -(int)tag );
                        } else {
                            int code;
                            if ( !TryParseHex( seg, out code ) ) {
                                Error( "Invalid code point '{0}'", seg );
                            }
                            _codeList.Add( code );
                        }
                    }
                }
                if ( _codeList.Count > 2 ) {
                    _entry._decomposingStart = -_codeList.Count;
                    _entry._decomposingStart2 = AddDecomposingCodes();
                } else {
                    _entry._decomposingStart = _codeList[ 0 ];
                    if ( _codeList.Count > 1 ) {
                        _entry._decomposingStart2 = _codeList[ 1 ];
                    }
                }
            }
        }
    }
}
