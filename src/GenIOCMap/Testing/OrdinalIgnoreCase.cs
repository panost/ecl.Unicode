using System;
namespace GenIOCMap {
    partial class OrdinalIgnoreCase {
        public const int CaseInsensitiveSeed = 5381;

        //internal static readonly uint CaseInsensitiveSeed = GetRandomUint( 5381 );
        //internal static readonly uint CaseSensitiveSeed = GetRandomUint( 0x15051505 );

        //private static uint GetRandomUint(int someSeed) {
        //    var rnd = new Random( Environment.TickCount ^ someSeed );
        //    return (uint)( rnd.Next() ^ rnd.Next() );
        //}

        public static char ToUpper( char ch ) {
            return _map.Map( ch );
        }

        internal static unsafe int GetCaseInsensitiveHashCode(char* str,int length){
            uint hash = CaseInsensitiveSeed;
            uint c;
            for ( ; length > 0; length--, str++ ) {
                c = *str;

                if (c - 'a' <= (uint)('z' - 'a')) {
                    c &= ~0x20u;
                } else if ( c >= 0x80 ) {
                    c = _map.Map( (char)c );
                }

                hash = ((hash << 5) + hash) ^ c;
            }
            return (int)hash;
        }

        internal static unsafe bool CaseInsensitiveEquals( char* a, char* b, int length ) {
            for ( ; length > 0; length--, a++, b++ ) {
                int av = *a;
                int bv = *b;
                switch ( av - bv ) {
                case 0:
                    continue;
                case 'A' - 'a': // -32
                    if ( bv >= 'a' && bv <= 'z' ) {
                        continue;
                    }
                    goto default;
                case 'a' - 'A': // 32
                    if ( av >= 'a' && av <= 'z' ) {
                        continue;
                    }
                    goto default;
                default:
                    if ( _map.Map( (char)av ) != _map.Map( (char)bv ) ) {
                        return false;
                    }
                    break;
                }
            }
            return true;
        }

        

        internal static unsafe int CaseInsensitiveCompare( char* a, char* b, int lengthA, int lengthB ) {
            int length = Math.Min(lengthA, lengthB);
            for ( ; length > 0; length--, a++, b++ ) {
                int av = *a;
                int bv = *b;
                switch ( av - bv ) {
                case 0:
                    continue;
                case 'A' - 'a': // -32
                    if ( bv >= 'a' && bv <= 'z' ) {
                        continue;
                    }
                    goto default;
                case 'a' - 'A': // 32
                    if ( av >= 'a' && av <= 'z' ) {
                        continue;
                    }
                    goto default;
                default:
                    int diff = _map.Map( (char)av ) - _map.Map( (char)bv );
                    if ( diff!=0 ) {
                        return diff;
                    }
                    break;
                }
            }

            return lengthA - lengthB;
        }

        public unsafe static int Compare( string left, string right ) {
            fixed ( char* a = left )
            fixed ( char* b = right )
                return CaseInsensitiveCompare( a, b, left.Length, right.Length );
        }

        public unsafe static bool EqualsTo( string x, string y ) {
            if ( ReferenceEquals( x, y ) )
                return true;
            if ( x == null )
                return y.Length==0;
            int length = x.Length;
            if ( y == null )
                return length == 0;
            if (length != y.Length){
                return false;
            }
            fixed ( char* a = x )
            fixed ( char* b = y ) {
                return CaseInsensitiveEquals( a, b, length );
            }
        }

        public unsafe static int GetHashCode( string str ) {
            if (str==null)
                return CaseInsensitiveSeed;
            fixed ( char* a = str ) {
                return GetCaseInsensitiveHashCode( a, str.Length );
            }
        }
    }
}
