using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GenIOCMap {
    class InterleaveMap {
        private HighMap[] _highMap;

        private MiddleMap[] _allMiddle;

        public InterleaveMap() {
            _highMap = new HighMap[ 256 ];
            _allMiddle = new MiddleMap[ 256 * 16 ];
            for ( int i = 0; i < _highMap.Length; i++ ) {
                var m = _highMap[ i ] = new HighMap();
                Array.Copy( m.Map, 0, _allMiddle, i * 16, 16 );
            }
        }

        int _maxOffset = int.MinValue;
        int _minOffset = int.MaxValue;

        public void Add( char wch, int diff ) {
            SetSlot( wch, diff );
        }
        public void AddSet( char wch, int diff ) {
            SetSlot( wch, diff ).Mask = -1;
        }

        private MiddleMap SetSlot( char wch, int diff ) {
            if ( diff > _maxOffset ) {
                _maxOffset = diff;
            }

            if ( diff < _minOffset ) {
                _minOffset = diff;
            }
            HighMap high = _highMap[ wch >> 8 ];
            MiddleMap mid = high.Map[ ( wch >> 4 ) & 0xf ];
            mid.Map[ wch & 0xf ] = diff;
            return mid;
        }

        class HighMap {
            public readonly MiddleMap[] Map;
            public int Index = -1;

            public HighMap() {
                Map = new MiddleMap[ 16 ];
                for ( int i = 0; i < Map.Length; i++ ) {
                    Map[ i ] = new MiddleMap();
                }
            }

            public bool Equals( HighMap other ) {
                for ( int i = 0; i < Map.Length; i++ ) {
                    if ( other.Map[ i ] != Map[ i ] ) {
                        return false;
                    }
                }

                return true;
            }
        }

        class MiddleMap {
            public readonly int[] Map;
            public int Index = -1;
            public int Mask;

            public MiddleMap() {
                Map = new int[ 16 ];
            }

            public bool Equals( MiddleMap other ) {
                for ( int i = 0; i < Map.Length; i++ ) {
                    if ( other.Map[ i ] != Map[ i ] ) {
                        return false;
                    }
                }

                return true;
            }
        }

        

        private List<MiddleMap> Calc(out List<HighMap> hiList) {
            List<MiddleMap> list = new List<MiddleMap>();

            for ( int i = 0; i < _allMiddle.Length; i++ ) {
                var mid = _allMiddle[ i ];
                if ( mid.Index < 0 ) {
                    mid.Index = list.Count;
                    list.Add( mid );
                }

                for ( int j = i + 1; j < _allMiddle.Length; j++ ) {
                    if ( mid.Equals( _allMiddle[ j ] ) ) {
                        _allMiddle[ j ].Index = mid.Index;
                    }
                }
            }

            foreach ( HighMap highMap in _highMap ) {
                for ( int i = 0; i < 16; i++ ) {
                    highMap.Map[ i ] = list[ highMap.Map[ i ].Index ];
                }
            }

            hiList = new List<HighMap>();
            for ( int i = 0; i < _highMap.Length; i++ ) {
                var high = _highMap[ i ];
                if ( high.Index < 0 ) {
                    high.Index = hiList.Count;
                    hiList.Add( high );
                }

                for ( int j = i + 1; j < _highMap.Length; j++ ) {
                    if ( high.Equals( _highMap[ j ] ) ) {
                        _highMap[ j ].Index = high.Index;
                    }
                }
            }

            return list;
        }
        public void Save( BinaryWriter writer ) {
            List<MiddleMap> list = Calc( out List<HighMap> hiList );

            writer.Write( (ushort)hiList.Count );
            writer.Write( (ushort)list.Count );
            for ( int i = 0; i < _highMap.Length; i++ ) {
                writer.Write( (ushort)( _highMap[ i ].Index * 16 ) );
            }
            for ( int i = 0; i < hiList.Count; i++ ) {
                var mid = hiList[ i ].Map;
                for ( int j = 0; j < 16; j++ ) {
                    writer.Write( (ushort)( mid[ j ].Index * 16 ) );
                }
            }
            for ( int i = 0; i < list.Count; i++ ) {

                int[] mid = list[ i ].Map;
                for ( int j = 0; j < 16; j++ ) {
                    writer.Write( (ushort)mid[ j ] );
                }
            }
        }
        public void SaveByte(BinaryWriter writer) {
            List<MiddleMap> list = Calc( out List<HighMap> hiList );

            writer.Write( (ushort)hiList.Count );
            writer.Write( (ushort)list.Count );
            for ( int i = 0; i < _highMap.Length; i++ ) {
                writer.Write( (ushort)( _highMap[ i ].Index * 16 ) );
            }
            for ( int i = 0; i < hiList.Count; i++ ) {
                var mid = hiList[ i ].Map;
                for ( int j = 0; j < 16; j++ ) {
                    writer.Write( (ushort)( mid[ j ].Index * 16 ) );
                }
            }
            for ( int i = 0; i < list.Count; i++ ) {

                int[] mid = list[ i ].Map;
                for ( int j = 0; j < 16; j++ ) {
                    byte bb = (byte)mid[ j ];
                    //if ( bb == 11 ) {
                    //    Debug.WriteLine( "Asdsad" );
                    //}
                    writer.Write( bb );
                }
            }
        }

        public void SaveMasked( BinaryWriter writer ) {
            List<MiddleMap> list = Calc( out List<HighMap> hiList );

            writer.Write( (ushort)hiList.Count );
            writer.Write( (ushort)list.Count );
            for ( int i = 0; i < _highMap.Length; i++ ) {
                writer.Write( (ushort)( _highMap[ i ].Index * 16 ) );
            }
            for ( int i = 0; i < hiList.Count; i++ ) {
                var mid = hiList[ i ].Map;
                for ( int j = 0; j < 16; j++ ) {
                    writer.Write( (ushort)( mid[ j ].Index * 16 ) );
                }
            }
            for ( int i = 0; i < list.Count; i++ ) {
                MiddleMap middle = list[ i ];
                int[] mid = middle.Map;
                writer.Write( (ushort)middle.Mask );
                Debug.WriteLine( $"mid {i}:{(ushort)middle.Mask:X4}" );
                for ( int j = 0; j < 16; j++ ) {
                    writer.Write( (ushort)mid[ j ] );
                }
            }
        }
        public void Write( TextWriter writer ) {

            List<MiddleMap> list = Calc( out List<HighMap> hiList );
            writer.WriteLine( "static CharMapper _map = new CharMapper( new byte[256] {" );
            for ( int i = 0; i < _highMap.Length; i++ ) {
                if ( i > 0 ) {
                    writer.Write( "," );
                    if ( ( i & 15 ) == 0 ) {
                        writer.WriteLine( "" );
                    }
                }

                writer.Write( _highMap[ i ].Index.ToString() );
            }

            writer.WriteLine( "}}, new byte[{0}*16] {{", hiList.Count );
            for ( int i = 0; i < hiList.Count; i++ ) {
                var mid = hiList[ i ].Map;
                for ( int j = 0; j < 16; j++ ) {
                    if ( j > 0 ) {
                        writer.Write( "," );
                    }

                    writer.Write( mid[ j ].Index.ToString() );
                }

                if ( i + 1 < hiList.Count ) {
                    writer.Write( "," );
                }

                writer.WriteLine( "" );
            }

            if ( _minOffset >= ushort.MinValue && _maxOffset <= ushort.MaxValue ) {
                writer.Write( "}}, new ushort[{0} * 16] {{", list.Count  );
            } else {
                writer.Write( "}}, new int[{0} * 16] {{", list.Count  );
            }

            writer.WriteLine( " // min:{0}, max:{1}", _minOffset, _maxOffset );
            for ( int i = 0; i < list.Count; i++ ) {
                var mid = list[ i ].Map;
                for ( int j = 0; j < 16; j++ ) {
                    if ( j > 0 ) {
                        writer.Write( "," );
                    }

                    writer.Write( mid[ j ].ToString() );
                }

                if ( i + 1 < list.Count ) {
                    writer.Write( "," );
                }

                writer.WriteLine( "" );
            }

            writer.WriteLine( "});" );
        }

    }
}
