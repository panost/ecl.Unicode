using System;

namespace eclUnicode.Ucd {
    /// <summary>
    /// Unicode code point range
    /// </summary>
    public class UcdRange : UcdContainer, IComparable<int>, IComparable<UcdRange> {
        public readonly int Begin;
        public readonly int End;

        public UcdRange( int begin, int end ) {
            Begin = begin;
            End = end;
        }

        protected UcdRange() {
            End = -1; // that will make Length == 0
        }
        public int CompareTo( int codePoint ) {
            int diff = Begin - codePoint;
            if ( diff <= 0
                 && codePoint <= End ) {
                return 0;
            }
            return diff;
        }

        public sealed override bool Contains( int codePoint ) {
            return CompareTo( codePoint ) == 0;
        }

        public int CompareTo( UcdRange other ) {
            int diff = Begin - other.Begin ;
            if ( diff == 0 ) {
                return End - other.End;
            }
            return diff;
        }

        public int Length {
            get {
                return End - Begin + 1;
            }
        }
        public override string ToString() {
            int len = Length;
            if ( len <= 0 ) {
                return "Empty";
            }
            if ( len == 1 ) {
                return Begin.ToString( "X4" );
            }
            return Begin.ToString( "X4" ) + ".." + End.ToString( "X4" );
        }

        
        public static int Find( UcdRange[] blocks, int index, int length, int codePoint ) {
            while( length > 0 ) {
                int mid = length / 2;
                int c = blocks[ index + mid ].CompareTo( codePoint );
                if( c == 0 )
                    return index + mid;
                if( c > 0 ) {
                    mid++;
                    index += mid;
                    length -= mid;
                } else {
                    length = mid;
                }
            }
            return ~index;
        }

        public static void Sort( UcdRange[] blocks ) {
            Array.Sort( blocks, ( a, b ) => a.CompareTo( b ) );
        }

        public static int IndexOfBlock( UcdRange[] blocks, int codePoint ) {
            return Find( blocks, 0, blocks.Length, codePoint );
        }

        public static T FindBlock<T>( T[] blocks, int codePoint ) where T : UcdRange {
            int index = IndexOfBlock( blocks, codePoint );
            if ( index >= 0 ) {
                return blocks[ index ];
            }
            return null;
        }


    }
}
