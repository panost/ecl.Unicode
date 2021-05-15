using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ecl.Unicode.Ucd {
    public readonly struct CodePointsBitmap {
        public const int BITMAP_LENGTH = ( (int)UnicodeCodePoint.Last + 1 ) / 8;

        private readonly byte[] _bitmap;

        public CodePointsBitmap( byte[] bitmap ) {
            _bitmap = bitmap;
        }
        public CodePointsBitmap( int maxValue ) {
            _bitmap = new byte[ ( maxValue + 1 ) / 8 ];
        }
        public CodePointsBitmap( bool all ) {
            _bitmap = new byte[ BITMAP_LENGTH ];
        }

        public int MaxValue => _bitmap?.Length * 8 - 1 ?? 0;

        public void Add( int codePoint ) {
            int index = ( codePoint & (int)UnicodeCodePoint.Last ) / 8;
            int offset = codePoint & 7;
            _bitmap[ index ] |= (byte)( 0x1U << offset );
        }
        public void Dec( int codePoint ) {
            int index = ( codePoint & (int)UnicodeCodePoint.Last ) / 8;
            int offset = codePoint & 7;
            _bitmap[ index ] &= (byte)~( 0x1U << offset );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool Contains( int codePoint ) {
            int index = codePoint / 8;
            if ( (uint)index < (uint)_bitmap.Length ) {
                int offset = codePoint & 7;
                return ( _bitmap[ index ] & ( 0x1U << offset ) ) != 0;
            }

            return false;
        }
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Add( int start, int end ) {
            for ( int i = start; i <= end; i++ ) {
                Add( i );
            }
        }
        public void Add( IEnumerable<UcdRange> ranges) {
            int maxValue = MaxValue;
            foreach ( var range in ranges ) {
                Add( range.Begin, Math.Min( range.End, maxValue ) );
            }
        }

        public void Add( UcdRange range ) {
            Add( range.Begin, range.End );
        }
    }
}
