using System;
namespace GenIOCMap {
    public struct CharMapper {
        private readonly byte[] _high;
        private readonly byte[] _mid;
        private readonly ushort[] _offsets;

        public CharMapper( byte[] high, byte[] mid, ushort[] offsets ) {
            _high = high;
            _mid = mid;
            _offsets = offsets;
        }

        public char Map( char wch ) {
            int idx = _mid[ ( _high[ wch >> 8 ] << 4 ) + ( ( wch >> 4 ) & 0xf ) ] << 4;
            //int high = _high[ wch >> 8 ]<< 4;
            //int mid = high + ( ( wch >> 4 ) & 0xf );
            //mid = _mid[ mid ]<<4;
            //int mid = _mid[ ( _high[ wch >> 8 ] << 4 ) + ( ( wch >> 4 ) & 0xf ) ];
            //int offset = _offsets[ mid + ( wch & 0xf ) ];
            //return (char)( offset ^ wch );
            return (char)( _offsets[ idx + ( wch & 0xf ) ] ^ wch );
        }
    }
}
