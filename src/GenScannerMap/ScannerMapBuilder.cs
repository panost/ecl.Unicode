using System;
using System.Collections.Generic;
using System.Text;
using ecl.Unicode.Ucd;

namespace GenScannerMap {
    class ScannerMapBuilder {
        private UcdLoader _loader;

        struct CodeEntry {
            public int CodePoint;
            public int Transform;

        }

        readonly List<CodeEntry> _list = new List<CodeEntry>();
        readonly List<int> _decomposing = new List<int>();

        int GetDecomposed(int codePoint, bool cascade) {
            _decomposing.Clear();
            _loader.AddDecomposing( codePoint, _decomposing,cascade );
            return _decomposing[ 0 ];
        }

        public ScannerMapBuilder( UcdLoader loader ) {
            _loader = loader;

            foreach ( UcdBlock block in loader.Blocks ) {
                switch ( block.Block ) {
                case Block.HighSurrogates:
                case Block.HighPrivateUseSurrogates:
                case Block.LowSurrogates:
                case Block.PrivateUseArea:
                case Block.SupplementaryPrivateUseAreaA:
                case Block.SupplementaryPrivateUseAreaB:
                    continue;
                }
                _list.Clear();
                foreach ( UnicodeEntry entry in loader.GetCodePoints( block ) ) {
                    CodeEntry code;
                    code.CodePoint = entry.CodeValue;
                    switch ( entry.Category ) {
                    case UnicodeCharacterType.LetterUppercase:
                    case UnicodeCharacterType.LetterLowercase:
                    case UnicodeCharacterType.LetterTitlecase:
                    case UnicodeCharacterType.LetterModifier:
                    case UnicodeCharacterType.LetterOther:
                        break;
                    default:
                        continue;
                    }

                    if ( entry.DecomposingLength > 0 ) {
                        int val = GetDecomposed( entry.CodeValue, true );

                    }
                }

            }

        }
    }
}
