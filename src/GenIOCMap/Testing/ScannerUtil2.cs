using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GenIOCMap.Testing {
    class ScannerUtil2 {
        private static MapPtr _uMap = MapPtr.Load();

        [StructLayout(LayoutKind.Sequential)]
        struct CompositeEnty {
            public ushort Code;
            public ushort Offset;
        }

        unsafe struct MapPtr {
            private ushort* _high;
            private ushort* _mid;
            private ushort* _offsets;
            private CompositeEnty* _composites;
            private ushort* _compositeData;
            private ushort CompositeCount;
#if DEBUG
            private uint _midSize;
            private uint _dataSize;
            [MethodImpl( MethodImplOptions.NoInlining )]
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
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            public char Map( char wch ) {
                int idx = _mid[ ( _high[ wch >> 8 ] ) + ( ( wch >> 4 ) & 0xf ) ] ;
                return (char)( _offsets[ idx + ( wch & 0xf ) ] ^ wch);
            }
#endif
            public static MapPtr Load() {
                ushort* ptr;
                using ( var o = (UnmanagedMemoryStream)typeof( MapPtr ).Assembly
                    .GetManifestResourceStream( typeof(ScannerMapBuilder).Namespace + ".Scanner2.bin" ) ) {
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
                ptr += dataSize;
                map.CompositeCount = *ptr;
                ++ptr;
                map._composites = (CompositeEnty*)ptr;
                ptr += map.CompositeCount * 2;
                map._compositeData = ptr;

#if DEBUG
                map._midSize = midSize;
                map._dataSize = dataSize;
#endif

                return map;
            }

            public ReadOnlySpan<char> GetComposite( char value ) {
                int length = CompositeCount;
                CompositeEnty* index = _composites;
                while ( length > 0 ) {
                    int mid = length / 2;
                    //Debug.WriteLine( index + " (" + ( index + mid ) + ") " + ( index + length - 1 ), "test" );
                    //int i = ( lo + hi ) >> 1;
                    int c = value - index[ mid ].Code;
                    if ( c == 0 ) {
                        ushort* ptr = _compositeData + index[ mid ].Offset;
                        int size = *ptr;
                        ptr++;
                        return new ReadOnlySpan<char>( ptr, size );
                    }

                    if ( c > 0 ) {
                        index += mid + 1;
                        length -= mid + 1;
                    } else {
                        length = mid;
                    }
                }
                return default;
            }
        }

        private static bool IsDisplayable( UnicodeCategory cat ) {
            switch ( cat ) {
            case UnicodeCategory.Control:
            case UnicodeCategory.NonSpacingMark:
            case UnicodeCategory.SpacingCombiningMark:
            case UnicodeCategory.EnclosingMark:
            case UnicodeCategory.SpaceSeparator:
            case UnicodeCategory.LineSeparator:
            case UnicodeCategory.ParagraphSeparator:
            case UnicodeCategory.Format:
            case UnicodeCategory.Surrogate:
            case UnicodeCategory.PrivateUse:
            case UnicodeCategory.ModifierSymbol:

                return false;
            }

            return true;
        }

        public static void TestNormalize(  ) {
            using ( var w = File.CreateText( @"f:\_tests\Del\Codes.txt" ) ) {
                for ( int i = 0; i < ushort.MaxValue; i++ ) {
                    char other = _uMap.Map( (char)i );
                    UnicodeCategory cat = char.GetUnicodeCategory( (char)i );
                    w.Write( $"0x{i:X4} {cat} " );
                    if ( IsDisplayable( cat ) ) {
                        w.Write( $"({char.ToString( (char)i )}) " );
                    }

                    if ( other < 32 ) {
                        if ( other != 0 ) {
                            if ( other < 10 ) {
                                w.WriteLine( $"Composite[]" );
                            } else {
                                w.WriteLine( $"Extra" );
                            }
                        } else {
                            w.WriteLine( $"Ignore" );
                        }
                    } else {
                        w.WriteLine( $"Normal" );
                    }
                }
            }
        }

        public static string Normalize( string text ) {
            var b = new StringBuilder();
            foreach ( char c in text ) {
                char other = _uMap.Map( c );
                if ( other < 32 ) {
                    if ( other != 0 ) {
                        if ( other == 1 ) {
                            var span = _uMap.GetComposite( c );
                            b.Append( span );
                        }
                    }
                } else {
                    b.Append( other );
                }
            }
            return b.ToString();
        }
    }
}
