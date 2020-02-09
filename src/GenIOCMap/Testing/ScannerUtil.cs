using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace GenIOCMap.Testing {
    static class ScannerUtil {
        private static MapPtr _uMap = MapPtr.Load();

        struct LigatureEntry {
            public ushort Key;
            public ushort Offset;
        }
        unsafe struct MapPtr {
            private int Ligatures;
            public ushort* _allCodes;
            private LigatureEntry* _ligatureTable;
            private ushort* _offsets;

            public static MapPtr Load() {
                byte* ptr;
                using ( var o = (UnmanagedMemoryStream)typeof( MapPtr ).Assembly
                    .GetManifestResourceStream( typeof(ScannerMapBuilder).Namespace + ".Scanner.bin" ) ) {
                    ptr = o.PositionPointer;
                }

                MapPtr map;

                map._allCodes = (ushort*)ptr;
                ptr += 65536*2;
                map.Ligatures = *(int*)ptr;
                ptr += 4;
                map._ligatureTable = (LigatureEntry*)ptr;
                ptr += map.Ligatures * 4;
                map._offsets = (ushort*)ptr;

                return map;
            }

            
        }

        public static unsafe void Translate( ReadOnlySpan<char> text, StringBuilder b ) {
            for ( int i = 0; i < text.Length; i++ ) {
                char ch = text[ i ];
                ushort val = _uMap._allCodes[ ch ];
                if ( val < 32 ) {
                    if ( val > 0 ) {
                        if ( val <= 5 ) {

                        }
                    }
                } else {
                    b.Append( (char)val );
                }
            }
        }
        public static string Normalize( string text ) {
            var b = new StringBuilder();
            Translate( text.AsSpan(), b );
            return b.ToString();
        }
    }
}
