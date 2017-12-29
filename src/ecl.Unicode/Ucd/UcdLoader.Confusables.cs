using System;
using System.Collections.Generic;
using System.IO;

namespace ecl.Unicode.Ucd {
    partial class UcdLoader {
        struct ConfusableEntry {
            public int Code;
            public int Index;
            public byte Length;
        }

        private ConfusableEntry[] _confusables;
        private int[] _confusableMap;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <remarks>
        /// http://www.unicode.org/reports/tr36/
        /// </remarks>
        public void LoadConfusables( TextReader reader ) {
            string line;
            char[] delim = { ';' };
            List<int> all = new List<int>();
            List<int> codeList = new List<int>();
            List< ConfusableEntry > entries = new List<ConfusableEntry>();
            while ( ( line = reader.ReadLine() ) != null ) {
                if ( line.Length == 0 || line[ 0 ] == '#' )
                    continue;
                string[] segs = line.Split( delim, 3 );
                if ( segs.Length != 3 )
                    continue;
                ConfusableEntry entry = new ConfusableEntry();
                if( !TryParseHex( segs[ 0 ], out entry.Code ) ) {
                    Error( "Invalid code value '{0}'", segs[ 0 ] );
                }
                ParseHexValues( codeList, segs[1] );
                entry.Length = (byte)codeList.Count;
                entry.Index = all.IndexOf( codeList );
                if ( entry.Index < 0 ) {
                    entry.Index = all.Count;
                    all.AddRange( codeList );
                }
                entries.Add( entry );
            }
            _confusables = entries.ToArray();
            _confusableMap = all.ToArray();
        }

        private bool ParseHexValues( List<int> codeList, string text ) {
            codeList.Clear();
            int conf;

            if ( !TryParseHex( text, out conf ) ) {
                var codes = text.Split( _spaceDelims, StringSplitOptions.RemoveEmptyEntries );
                for ( int i = 0; i < codes.Length; i++ ) {
                    if ( !TryParseHex( codes[ i ], out conf ) ) {
                        Error( "Invalid code value '{0}'", codes[ i ] );
                        return false;
                    }
                    codeList.Add( conf );
                }
            } else {
                codeList.Add( conf );
            }
            return true;
        }
        private bool TryParseHex( List<int> codeList, string text, out Values<int> value) {
            if ( !ParseHexValues( codeList, text ) ) {
                value = default;
                return false;
            }
            switch ( codeList.Count ) {
            case 0:
                value = default;
                break;
            case 1:
                value = new Values<int>( codeList[ 0 ] );
                break;
            default:
                value = new Values<int>( codeList.ToArray() );
                break;
            }
            return true;
        }

        public void LoadConfusables( string fileName ) {
            using( var reader = File.OpenText( fileName ) ) {
                LoadConfusables( reader );
            }
        }
    }
}
