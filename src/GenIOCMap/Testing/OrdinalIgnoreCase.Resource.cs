using System;
using System.IO;

namespace GenIOCMap {
    partial class OrdinalIgnoreCase {
        private static MapPtr _uMap = MapPtr.Load();

        internal static char ToUpper2( char ch ) {
            return _uMap.Map( ch );
        }
        unsafe struct MapPtr {
            private ushort* _high;
            private ushort* _mid;
            private ushort* _offsets;
#if DEBUG
            private uint _midSize;
            private uint _dataSize;
            public char Map( char wch ) {
                ushort high = _high[ wch >> 8 ] ;
                uint mid = high + (uint)( ( wch >> 4 ) & 0xf );
                if ( mid < _midSize ) {
                    mid = _mid[ mid ]+ (uint)( wch & 0xf );
                    if ( mid < _dataSize ) {
                        return (char)( _offsets[ mid ] ^ wch );
                    }
                }
                throw new IndexOutOfRangeException();
            }
#else
            public char Map( char wch ) {
                int idx = _mid[ ( _high[ wch >> 8 ] ) + ( ( wch >> 4 ) & 0xf ) ] ;
                return (char)( _offsets[ idx + ( wch & 0xf ) ] ^ wch );
            }
#endif
            public static MapPtr Load() {
                ushort* ptr;
                using ( var o = (UnmanagedMemoryStream)typeof( MapPtr ).Assembly
                    .GetManifestResourceStream( typeof(OrdinalIgnoreCase).FullName + ".bin" ) ) {
                    ptr = (ushort*)o.PositionPointer;
                }

                uint midSize = *ptr++ * 16u;
                uint dataSize = *ptr++ * 16u;

                MapPtr map;

                map._high = ptr;
                ptr += 256;
                map._mid = ptr;
                ptr += midSize;
                map._offsets = ptr;
#if DEBUG
                map._midSize = midSize;
                map._dataSize = dataSize;
#endif

                return map;
            }
        }
    }
}
