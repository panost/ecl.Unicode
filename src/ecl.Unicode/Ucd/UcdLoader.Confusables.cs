using System;
using System.Collections.Generic;
using System.IO;
using ecl.Unicode;

namespace eclUnicode.Ucd {
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
                codeList.Clear();
                int conf;
                if ( !TryParseHex( segs[ 1 ], out conf ) ) {
                    var codes = segs[ 1 ].Split( _spaceDelims, StringSplitOptions.RemoveEmptyEntries );
                    for ( int i = 0; i < codes.Length; i++ ) {
                        if ( !TryParseHex( codes[ i ], out conf ) ) {
                            Error( "Invalid code value '{0}'", codes[ i ] );
                        }
                        codeList.Add( conf );
                    }
                } else {
                    codeList.Add( conf );
                }
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

        public void LoadConfusables( string fileName ) {
            using( var reader = File.OpenText( fileName ) ) {
                LoadConfusables( reader );
            }
        }
    }
}
