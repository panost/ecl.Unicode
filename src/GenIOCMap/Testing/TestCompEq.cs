using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ecl.Unicode.Ucd;

namespace GenIOCMap.Testing {
    class TestCompEq {
        private UcdLoader _loader;
        private string[] _allStrings;
        private int[] _allPoints;

        public TestCompEq( UcdLoader loader ) {
            _loader = loader;
            List<string> list = new List<string>();
            List<int> listPoint = new List<int>();
            foreach ( var point in loader.GetCodePoints() ) {
                if ( point.CodeValue == 0
                     || ( point.CodeValue >= 0xD800 && point.CodeValue <= 0xDFFF ) ) {
                    continue;
                }
                list.Add( point.ToString() );
                listPoint.Add( point.CodeValue );
            }
            _allStrings = list.ToArray();
            _allPoints = listPoint.ToArray();
        }

        public void TestCompare() {
            for ( int i = 0; i < _allStrings.Length; i++ ) {
                if ( ( i & 0xff ) == 0 ) {
                    Debug.WriteLine( $"{i:X6} {i*100.0/_allStrings.Length}%" );
                }
                string left = _allStrings[ i ];
                for ( int j = i+1; j < _allStrings.Length; j++ ) {
                    string right = _allStrings[ j ];

                    int cmp = string.Compare( left, right, StringComparison.OrdinalIgnoreCase );
                    int cmp2 = OrdinalIgnoreCase.Compare( left, right );
                    if ( Math.Sign( cmp )!=Math.Sign( cmp2 )) {
                        Console.WriteLine( "asd" );
                    }
                }
            }
        }

        public void TestMaps() {
            for ( int i = 0; i < _allStrings.Length; i++ ) {
                if ( ( i & 0xff ) == 0 ) {
                    Debug.WriteLine( $"{i:X6} {i*100.0/_allStrings.Length}%" );
                }
                string left = _allStrings[ i ];
                for ( int j = i+1; j < _allStrings.Length; j++ ) {
                    string right = _allStrings[ j ];

                    int cmp = string.Compare( left, right, StringComparison.OrdinalIgnoreCase );
                    int cmp2 = OrdinalIgnoreCase.Compare( left, right );
                    if ( Math.Sign( cmp )!=Math.Sign( cmp2 )) {
                        Console.WriteLine( "asd" );
                    }
                }
            }
        }
        public void TestEquals() {
            for ( int i = 0; i < _allStrings.Length; i++ ) {
                if ( ( i & 0xff ) == 0 ) {
                    Debug.WriteLine( $"{i:X6} {i*100.0/_allStrings.Length}%" );
                }
                string left = _allStrings[ i ];
                for ( int j = i+1; j < _allStrings.Length; j++ ) {
                    string right = _allStrings[ j ];

                    bool cmp = string.Equals( left, right, StringComparison.OrdinalIgnoreCase );
                    bool cmp2 = OrdinalIgnoreCase.EqualsTo( left, right );
                    if ( cmp != cmp2 ) {
                        Console.WriteLine( "asd" );
                    }
                }
            }
        }
    }
}
