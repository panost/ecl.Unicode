using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ecl.Unicode.Ucd {
    partial class UcdLoader {
        private CaseFoldingEntry[] _caseFoldings;
        /// <summary>
        /// 
        /// </summary>
        public CaseFoldingEntry[] CaseFoldings {
            get {
                if ( _caseFoldings == null ) {
                    _caseFoldings = GetCaseFolding();
                }
                return _caseFoldings;
            }
        }

        private CaseFoldingEntry[] GetCaseFolding() {
            List<CaseFoldingEntry> list = new List<CaseFoldingEntry>();
            List<string> segs = new List<string>();
            List<int> codeList = new List<int>();

            using ( LineReader reader = OpenLineReader( "CaseFolding.txt" ) ) {
                foreach ( var count in reader.GetLines( segs, 3 ) ) {
                    var entry = new CaseFoldingEntry();
                    if ( !TryParseHex( segs[ 0 ], out entry.Code ) ) {
                        continue;
                    }
                    var status = segs[ 1 ].Trim();
                    if ( status.Length != 1 )
                        continue;
                    switch ( status[ 0 ] ) {
                    case 'C':
                        entry.Status = CaseFoldingStatus.Common;
                        break;
                    case 'F':
                        entry.Status = CaseFoldingStatus.Full;
                        break;
                    case 'S':
                        entry.Status = CaseFoldingStatus.Simple;
                        break;
                    case 'T':
                        entry.Status = CaseFoldingStatus.Special;
                        break;
                    default:
                        continue;
                    }
                    if ( TryParseHex( codeList, segs[ 2 ], out entry.Mapping ) ) {
                        list.Add( entry );
                    }
                }
            }
            list.Sort();
            return list.ToArray();
        }

        public enum CaseFoldingStatus {
            /// <summary>
            /// common case folding, common mappings shared by both simple and full mappings.
            /// </summary>
            Common,

            /// <summary>
            /// full case folding, mappings that cause strings to grow in length.
            /// Multiple characters are separated by spaces.
            /// </summary>
            Full,

            /// <summary>
            /// simple case folding, mappings to single characters where different from F.
            /// </summary>
            Simple,

            /// <summary>
            /// special case for uppercase I and dotted uppercase I
            /// </summary>
            Special
        }
        public struct CaseFoldingEntry : IComparable<CaseFoldingEntry>, IComparable<int> {
            public Values<int> Mapping;
            public int Code;
			public CaseFoldingStatus Status;
            public int CompareTo( CaseFoldingEntry other ) {
                int cmp = Code.CompareTo( other.Code );
                if ( cmp == 0 )
                    return (int)Status - (int)other.Status;
                return cmp;
            }

            public int CompareTo( int other ) {
                return Code.CompareTo( other );
            }
			public override string ToString() {
				return $"U+{Code:X4} '{(char)Code}' -> [{string.Join( ",", Mapping )}] ({Status})";
			}
        }
		public ArraySegment<CaseFoldingEntry> GetFoldingCharacters( int codeValue ) {
			int idx = CaseFoldings.BinaryFind( codeValue );
			idx = CaseFoldings.GetRange( codeValue, out int stop );
			if ( idx >= 0 ) {
				return new ArraySegment<CaseFoldingEntry>( _caseFoldings, idx, stop - idx + 1 );
			}
			return default;
		}
		public int GetFoldingCharacter( int codeValue ) {
            int idx = CaseFoldings.BinaryFind( codeValue );
            idx = CaseFoldings.GetRange( codeValue, out int stop );
            if ( idx >= 0 ) {
                for ( int i = idx; i <= stop; i++ ) {
                    ref CaseFoldingEntry ptr = ref _caseFoldings[ i ];
                    if ( ptr.Mapping.Count==1
                        && ptr.Status == CaseFoldingStatus.Common
                         || ptr.Status == CaseFoldingStatus.Simple ) {
                        return ptr.Mapping.First;
                    }
                }
            }
            return -1;
        }
    }
}
