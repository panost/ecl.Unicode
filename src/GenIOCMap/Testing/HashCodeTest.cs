using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ecl.Unicode.Ucd;

namespace GenIOCMap.Testing {
    class HashCodeTest {
        private UcdLoader _loader;
        private string[] _allStrings;
        private int[][] _allPoints;
        public HashCodeTest( UcdLoader loader ) {
            _loader = loader;
            List<string> list = new List<string>();
            List<int> listPoint = new List<int>();
            List<int[]> listPoints = new List<int[]>();
            StringBuilder b = new StringBuilder();
            int len = 0;
            foreach ( var point in loader.GetCodePoints() ) {
                if ( point.CodeValue == 0 )
                    continue;
                point.AppendCharTo( b );
                listPoint.Add( point.CodeValue );
                len++;
                if ( len == 10 ) {
                    len = 0;
                    list.Add( b.ToString() );
                    listPoints.Add( listPoint.ToArray() );
                    b.Clear();
                    listPoint.Clear();
                }
            }

            if ( b.Length > 0 ) {
                list.Add( b.ToString() );
                listPoints.Add( listPoint.ToArray() );
            }

            _allStrings = list.ToArray();
            _allPoints = listPoints.ToArray();
        }


        public void TestHashCode() {
            for ( var i = 0; i < _allStrings.Length; i++ ) {
                var str = _allStrings[ i ];
                int hc = StringComparer.OrdinalIgnoreCase.GetHashCode( str );
                int hc2 = OrdinalIgnoreCase.GetHashCode( str );
                if ( hc != hc2 ) {
                    var points = _allPoints[ i ];

                    for ( int j = 0; j < points.Length; j++ ) {
                        UnicodeEntry cd = _loader[ points[ j ] ];
                        string str2 = cd.ToString();
                        hc = StringComparer.OrdinalIgnoreCase.GetHashCode( str2 );
                        hc2 = OrdinalIgnoreCase.GetHashCode( str2 );
                        if ( hc != hc2 ) {
                            if ( cd.Uppercase != 0 ) {
                                var cd2 = _loader[ cd.Uppercase ];
                                var str3 = cd2.ToString();
                                int hc3 = StringComparer.OrdinalIgnoreCase.GetHashCode( str3 );
                                //int hc4 = MapUtil.GetCaseInsensitiveHashCode( str3 );
                                if ( string.Equals( str2, str3, StringComparison.OrdinalIgnoreCase ) ) {
                                    Debug.WriteLine( "'{0}'", str2 );
                                }

                                if ( hc3 == hc ) {
                                    Debug.WriteLine( "'{0}' != '{1}' (U+{2:X6}) != U+{3:X6})" +
                                                     "(U+{4:X4},{5:X4}) != U+{6:X4},{7:X4})", 
                                        str2, str3, cd.CodeValue, cd2.CodeValue,
                                        (int)str2[0],(int)str2[1],(int)str3[0],(int)str3[1]);
                                }
                            } else {
                                Debug.WriteLine( "'{0}'", str2 );
                            }
                        }
                    }
                }
            }
        }
    }
}
