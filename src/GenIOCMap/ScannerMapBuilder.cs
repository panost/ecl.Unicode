using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ecl.Unicode;
using ecl.Unicode.Ucd;

namespace GenIOCMap {
    class ScannerMapBuilder {
        private UcdLoader _loader;

        public ScannerMapBuilder( UcdLoader loader ) {
            _loader = loader;
        }
        private void EnsureCodeMapBuild() {
            if ( _codeMap == null ) {
                _codeMap = new Dictionary<ushort, ushort>();
                BuildCodeMap();
            }
        }

        private Dictionary<ushort, ushort> _codeMap;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<ushort, ushort> CodeMap {
            get {
                EnsureCodeMapBuild();
                return _codeMap;
            }
        }


        readonly List<int> _decomposing = new List<int>();
        readonly List<UnicodeEntry> _decomposeLetters = new List<UnicodeEntry>();

        private static bool IsPrintable( UnicodeCharacterType cat ) {
            switch ( cat ) {
            case UnicodeCharacterType.OtherControl:
            case UnicodeCharacterType.LetterModifier:
            //case UnicodeCharacterType.SymbolModifier:
            case UnicodeCharacterType.OtherSurrogate:
                return false;
            }

            return true;
        }

        private static bool IsLetterOrDigit( UnicodeCharacterType cat ) {
            switch ( cat ) {
            case UnicodeCharacterType.LetterUppercase:
            case UnicodeCharacterType.LetterLowercase:

            case UnicodeCharacterType.LetterTitlecase:
            //case UnicodeCharacterType.LetterModifier:
            case UnicodeCharacterType.LetterOther:

            case UnicodeCharacterType.NumberDecimalDigit:
            case UnicodeCharacterType.NumberLetter:
            case UnicodeCharacterType.NumberOther:

                return true;
            }

            return false;
        }
        StringBuilder _builder = new StringBuilder();

        class CodeCompositeEntry : IEquatable<CodeCompositeEntry> {
            public readonly int Offset;
            public readonly int[] Composites;

            public CodeCompositeEntry( int compositeOffset, int[] composites ) {
                Offset = compositeOffset;
                Composites = composites;
            }

            public bool Equals( CodeCompositeEntry other ) {
                return Composites.AsSpan().SequenceEqual( other.Composites );
            }

            #region Overrides of Object

            public override int GetHashCode() {
                int hc = 0;
                for ( int i = 0; i < Composites.Length; i++ ) {
                    hc += ( hc << 4 ) + Composites[ i ];
                }
                return hc;
            }

            #endregion
        }
        readonly Dictionary<CodeCompositeEntry,CodeCompositeEntry> _composites = new Dictionary<CodeCompositeEntry, CodeCompositeEntry>();
        readonly List<int[]> _allComposites = new List<int[]>();
        private int _compositeOffset;
        private CodeCompositeEntry Add( int[] composites ) {
            var c = new CodeCompositeEntry( _compositeOffset, composites );
            if ( _composites.TryGetValue( c, out var found ) ) {
                return found;
            }

            _composites.Add( c, c );
            _compositeOffset += composites.Length + 1;
            _allComposites.Add( composites );
            return c;
        }
        readonly Dictionary<int,CodeCompositeEntry> _compositeMap = new Dictionary<int, CodeCompositeEntry>();

        private KeyValuePair<int, CodeCompositeEntry>[]  GetSortedComposites() {
            KeyValuePair<int, CodeCompositeEntry>[] pairs = _compositeMap.ToArray();
            Array.Sort( pairs, ( a, b ) => a.Key.CompareTo( b.Key ) );
            return pairs;
        }
        int GetDecomposed( ref UnicodeEntry entry, int orgValue ) {
            if ( _compositeMap.TryGetValue( entry.CodeValue, out var comp ) ) {
                if ( !_compositeMap.ContainsKey( orgValue ) ) {
                    _compositeMap.Add( orgValue, comp );
                }
                return 1;
            }
            _decomposing.Clear();
            _loader.AddDecomposing( entry.CodeValue, _decomposing, true );
            _decomposeLetters.Clear();
            foreach ( var c in _decomposing ) {
                if ( _loader.TryGetEntry( c, out UnicodeEntry decomposed ) ) {
                    if ( IsLetterOrDigit( decomposed.Category ) ) {
                        if ( decomposed.Uppercase != 0 ) {
                            if ( !_loader.TryGetEntry( decomposed.Uppercase, out decomposed )
                            || !IsLetterOrDigit( decomposed.Category ) ) {
                                continue;
                            }
                        }
                        _decomposeLetters.Add( decomposed );
                    }
                }
            }
            
            switch ( _decomposeLetters.Count ) {
            case 0:
                return 0;
            case 1:
                return _decomposeLetters[ 0 ].CodeValue;
            default:
                _builder.Clear();
                _decomposing.Clear();
                foreach ( var letter in _decomposeLetters ) {
                    _decomposing.Add( letter.CodeValue );
                    _builder.Append( ' ' ).Append( letter.ToString() );
                }

                _compositeMap.Add( entry.CodeValue, Add( _decomposing.ToArray() ) );

                //Debug.WriteLine( $"{entry.Name} ({entry}) d[{_decomposeLetters.Count}]:{_builder.ToString()}" );
                return 1;
            }
        }

        enum MoreWord : byte {
            Control=20,
            Punctuation,
            Symbol,
            WhiteSpace,
            LineSeparator,
            Connector,
            Ideograph,
            Surrogate,
            Private
        }
        public void WriteBreakMap() {
            EnumRange<WordBreak>[] breaks = _loader.LoadWordBreak();
            var map = new Dictionary<int,byte>();
            foreach ( var range in breaks ) {
                //switch ( range.Value ) {
                //case WordBreak.MidLetter:
                //    continue;
                //}
                for ( int i = range.Begin; i <= range.End; i++ ) {
                    if ( !map.ContainsKey( i ) ) {
                        map[ i ] = (byte)range.Value;
                    } else {
                        Debug.WriteLine( "DF" );
                    }
                }
            }
            
            map.Add( 160, (byte)MoreWord.Connector ); // Non Breaking space
            map.Add( '\t', (byte)WordBreak.WSegSpace ); // Tab

            foreach ( var entry in _loader.GetCodePoints(0,ushort.MaxValue) ) {
                if ( !map.ContainsKey( entry.CodeValue ) ) {
                    switch ( entry.Category ) {
                    case UnicodeCharacterType.OtherPrivateUse:
                        map.Add( entry.CodeValue, (byte)MoreWord.Private );
                        break;
                    case UnicodeCharacterType.OtherSurrogate:
                        map.Add( entry.CodeValue, (byte)MoreWord.Surrogate );
                        break;
                    case UnicodeCharacterType.OtherControl:
                        map.Add( entry.CodeValue, (byte)MoreWord.Control );
                        continue;
                    case UnicodeCharacterType.LetterUppercase:
                    case UnicodeCharacterType.LetterLowercase:
                    case UnicodeCharacterType.LetterTitlecase:
                        map.Add( entry.CodeValue, 52 );
                        break;
                    case UnicodeCharacterType.LetterOther:
                        map.Add( entry.CodeValue, (byte)MoreWord.Ideograph );
                        break;
                    case UnicodeCharacterType.NumberOther:
                    case UnicodeCharacterType.LetterModifier:
                        map.Add( entry.CodeValue, (byte)MoreWord.Symbol );
                        break;

                    case UnicodeCharacterType.NumberDecimalDigit:
                    case UnicodeCharacterType.MarkEnclosing:
                        map.Add( entry.CodeValue, (byte)MoreWord.Symbol );
                        break;
                    case UnicodeCharacterType.NumberLetter:
                    case UnicodeCharacterType.SymbolMath:
                    case UnicodeCharacterType.SymbolCurrency:
                    case UnicodeCharacterType.SymbolModifier:
                    case UnicodeCharacterType.SymbolOther:
                        map.Add( entry.CodeValue, (byte)MoreWord.Symbol );
                        break;
                    case UnicodeCharacterType.PunctuationConnector:
                        map.Add( entry.CodeValue, (byte)MoreWord.Punctuation );
                        break;
                    case UnicodeCharacterType.PunctuationDash:
                    case UnicodeCharacterType.PunctuationOpen:
                    case UnicodeCharacterType.PunctuationClose:
                    case UnicodeCharacterType.PunctuationInitialQuote:
                    case UnicodeCharacterType.PunctuationFinalQuote:
                    case UnicodeCharacterType.PunctuationOther:
                        map.Add( entry.CodeValue, (byte)MoreWord.Punctuation );
                        break;
                    case UnicodeCharacterType.OtherFormat:
                    case UnicodeCharacterType.SeparatorSpace:
                        map.Add( entry.CodeValue, (byte)MoreWord.WhiteSpace );
                        break;
                    case UnicodeCharacterType.MarkSpacingCombining:
                    case UnicodeCharacterType.SeparatorLine:
                    case UnicodeCharacterType.SeparatorParagraph:
                        map.Add( entry.CodeValue, (byte)MoreWord.LineSeparator );
                        break;
                    default:
                        map.Add( entry.CodeValue, 60 );
                        break;
                    }
                }
            }

            #if true1
            using ( var w = File.CreateText( @"f:\_tests\Del\WordBreak.txt" ) ) {
                for ( int i = 0; i <= ushort.MaxValue; i++ ) {
                    string text = $"{i:X4} ";
                    if ( _loader.TryGetEntry( i, out UnicodeEntry entry ) ) {
                        if ( IsPrintable( entry.Category ) ) {
                            text += entry + " ";
                        }

                        text += entry.Name + " ";
                    }
                    map.TryGetValue( i, out var value );
                    w.WriteLine( text + value );
                }
            }
#else
            InterleaveMap imap = new InterleaveMap();
            for ( int i = 0; i <= ushort.MaxValue; i++ ) {
                if ( map.TryGetValue( i, out byte value ) ) {
                    imap.Add( (char)i, value );
                }
            }


            using ( var w = File.Create( @"f:\_tests\Del\WordBreak2.bin" ) )
            using ( var b = new BinaryWriter( w ) ){
                imap.SaveByte( b );
            }
            #endif
        }

        private void BuildCodeMap() {
            foreach ( UcdBlock block in _loader.Blocks ) {
                switch ( block.Block ) {
                case Block.HighSurrogates:
                case Block.HighPrivateUseSurrogates:
                case Block.LowSurrogates:
                case Block.PrivateUseArea:
                case Block.SupplementaryPrivateUseAreaA:
                case Block.SupplementaryPrivateUseAreaB:
                    continue;
                }
                foreach ( UnicodeEntry i in _loader.GetCodePoints( block ) ) {
                    if ( i.CodeValue < 16 ) {
                        _codeMap.Add( (ushort)i.CodeValue, 0 );
                        continue;
                    }

                    if ( i.CodeValue == 'ὐ' ) {
                        Debug.WriteLine( "ὐ" );
                    }
                    uint value = (uint)i.CodeValue;
                    UnicodeEntry entry = i;
                    if ( entry.Uppercase != 0 ) {
                        value = (uint)entry.Uppercase;
                        if ( value > 0xFFFF )
                            continue;
                        if ( !_loader.TryGetEntry( entry.Uppercase, out entry ) ) {
                            continue;
                        }
                    }

                    if ( entry.DecomposingLength > 0 ) {
                        //if ( entry.CodeValue == 0x321D ) {
                        //    Debug.WriteLine( "Asdasd" );
                        //}
                        //SupportDecomposing(entry.Decomposing)
                        int control = GetDecomposed( ref entry, i.CodeValue );
                        if ( control != 0 ) {
                            //if ( !_loader.TryGetEntry( control, out entry ) || entry.CodeValue != control ) {
                            //    continue;
                            //}
                            //value = (uint)entry.CodeValue;
                            value = (uint)control;
                        }
                    }

                    //if ( value > 0xFFFF || i.CodeValue > 0xffff ) {
                    //    continue;
                    //}
                    if ( value != i.CodeValue ) {
                        _codeMap.Add( (ushort)i.CodeValue, (ushort)value );
                    }

                    //_list.Add( code );
                }

            }
        }
        

        private static bool SupportDecomposing( DecomposingTag tag ) {
            switch ( tag ) {
            case DecomposingTag.Compat:
                return false;
            }

            return true;
        }

        private void WriteComposites( BinaryWriter b ) {
            var pairs = GetSortedComposites();
            b.Write( (ushort)pairs.Length );
            for ( int i = 0; i < pairs.Length; i++ ) {
                var pair = pairs[ i ];
                b.Write( (ushort)pair.Key );
                b.Write( (ushort)pair.Value.Offset );
                //Debug.WriteLine( $"CO:{pair.Key},{pair.Value.Offset},{pair.Value.Composites.Length}:[{string.Join( ',', pair.Value.Composites )}]" );
            }

            //b.Write( (ushort)_allComposites.Count );
            foreach ( int[] composites in _allComposites ) {
                b.Write( (ushort)composites.Length );
                foreach ( int composite in composites ) {
                    b.Write( (ushort)composite );

                }

            }
        }

        public void WriteCodes() {
            EnsureCodeMapBuild();
            using ( var w = File.Create( @"F:\Dev\GitHub\ecl.Unicode\src\GenIOCMap\Scanner.bin" ) )
            using ( var b = new BinaryWriter( w, Encoding.UTF8, true ) ) {
                for ( int i = 0; i <= ushort.MaxValue; i++ ) {
                    ushort code = (ushort)i;
                    _codeMap.TryGetValue( code, out ushort value );
                    b.Write( value );
                }
                WriteComposites( b );
            }
        }
        public void WriteCodes3() {
            EnsureCodeMapBuild();
            var map = new InterleaveMap();
            for ( int i = 0; i <= ushort.MaxValue; i++ ) {
                if ( _codeMap.TryGetValue( (ushort)i, out ushort value ) && value >= 32 ) {
                    map.AddSet( (char)i, i ^ value );
                } else {
                    map.Add( (char)i, 0 );
                }
            }
            
            using ( var w = File.Create( @"F:\Dev\GitHub\ecl.Unicode\src\GenIOCMap\Scanner2.bin" ) )
            using ( var b = new BinaryWriter( w ) ) {
                map.SaveMasked( b );
                WriteComposites( b );
            }
        }

        enum ScriptSystem {
            Unknown,
            Common,
            Latin,
            Greek,
            Cyrillic,
            Georgian,
            Armenian,
            Hebrew,
            Arabic,
        }

        private static ScriptSystem GetScriptSystem( WritingScript cat ) {
            switch ( cat ) {
            case WritingScript.Common:
                return ScriptSystem.Common;
            case WritingScript.Latin:
                return ScriptSystem.Latin;
            case WritingScript.Cyrillic:
                return ScriptSystem.Cyrillic;
            case WritingScript.Greek:
                return ScriptSystem.Greek;
            case WritingScript.Armenian:
                return ScriptSystem.Armenian;
            case WritingScript.Georgian:
                return ScriptSystem.Georgian;

            case WritingScript.Arabic:
                return ScriptSystem.Arabic;
            case WritingScript.Hebrew:
                return ScriptSystem.Hebrew;
            }
            return 0;
        }

        public void WriteScriptMap() {
            _loader.EnsureDataLoaded();
            _loader.EnsureScriptsLoaded();
            EnsureCodeMapBuild();
            var map = new Dictionary<int, ScriptSystem>();


            foreach ( var entry in _loader.GetCodePoints( 0, ushort.MaxValue ) ) {
                if ( entry.Script != 0 ) {
                    ScriptSystem sys = GetScriptSystem( entry.Script );
                    map.Add( entry.CodeValue, sys );
                }
            }

#if true1

            using ( var w = File.CreateText( @"f:\_tests\Del\ScriptMap.txt" ) ) {
                for ( int i = 0; i <= ushort.MaxValue; i++ ) {
                    if ( map.TryGetValue( i, out ScriptSystem value ) ) {
                    }

                    string text = $"{i:X4} ";
                    if ( _loader.TryGetEntry( i, out UnicodeEntry entry ) ) {
                        if ( IsPrintable( entry.Category ) ) {
                            text += entry + " ";
                        }

                        if ( entry.Script != 0 ) {
                            text += "[" + entry.Script + "] ";
                        }
                        text += entry.Name + " ";
                    }

                    w.WriteLine( text + value );
                }
            }
#else
            InterleaveMap imap = new InterleaveMap();
            for ( int i = 0; i <= ushort.MaxValue; i++ ) {
                if ( map.TryGetValue( i, out ScriptSystem value ) ) {
                    imap.Add( (char)i, (int)value );
                }
            }


            using ( var w = File.Create( @"f:\_tests\Del\ScriptMap.bin" ) )
            using ( var b = new BinaryWriter( w ) ){
                imap.SaveByte( b );
            }
#endif
        }

        public void WriteCodes2() {
            EnsureCodeMapBuild();
            //int tot = 0;
            //foreach ( var pair in _codeMap ) {
            //    if ( pair.Key != pair.Value ) {
            //        Debug.WriteLine( $"{pair.Key:X4}:{pair.Value:X4}" );
            //        tot++;
            //    }
            //}

            var map = new InterleaveMap();
            for ( int i = 0; i <= ushort.MaxValue; i++ ) {
                if ( _codeMap.TryGetValue( (ushort)i, out ushort value ) ) {
                    map.Add( (char)i, i ^ value );
                }
            }
            
            using ( var w = File.Create( @"F:\Dev\GitHub\ecl.Unicode\src\GenIOCMap\Scanner2.bin" ) )
            using ( var b = new BinaryWriter( w ) ) {
                map.Save( b );
                WriteComposites( b );
            }
        }
    }
}
