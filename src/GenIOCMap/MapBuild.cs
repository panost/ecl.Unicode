using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using ecl.Unicode.Ucd;

namespace GenIOCMap {
    class MapBuild {
        private UcdLoader _loader;
        private Dictionary<int, int> _iocMap;
        public MapBuild(UcdLoader loader) {
            _loader = loader;
            loader.EnsureDataLoaded();
            var iocMap =_iocMap= new Dictionary<int, int>();
			
            foreach ( var line in File.ReadLines( @"../../../bin/OrdinalIgnoreCase.txt" ) ) {
                var s = line.Split( new[] { ' ' }, 3 );
                if ( int.TryParse( s[ 0 ], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int from )
                     && int.TryParse( s[ 1 ], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int to )) {

					if ( iocMap.TryGetValue( from, out int other ) ) {
						if ( ShouldChange( from, other, to ) ) {
							iocMap[ from ] = to;
						}
					} else {
						iocMap.Add( from, to );
					}
                }
            }

        }
		private bool ShouldChange( int from, int old, int to ) {
			UnicodeEntry oldEntry = _loader[ old ];
			switch ( oldEntry.Category ) {
			case UnicodeCharacterType.LetterUppercase:
			case UnicodeCharacterType.LetterTitlecase:
				return false;
			case UnicodeCharacterType.LetterLowercase:
				return true;
				//break;
			}
			Debug.WriteLine( $"hh" );
			return true;
		}
		public void BuildToUpperMap(bool text=false) {
            var map = new InterleaveMap();

            foreach ( KeyValuePair<int, int> pair in _iocMap ) {
                if ( !_iocMap.TryGetValue( pair.Value, out int key ) ||
                     key != pair.Key ) {
                    Console.WriteLine( "{0:X6} != {1:X6}", key, pair.Key );
                } else {
                    UnicodeEntry left = _loader[ pair.Key ];
                    //var right = _loader[ pair.Value ];
                    if ( left.Uppercase != 0 ) {
                        if ( left.Uppercase != pair.Value ) {
                            Console.WriteLine( "{0:X6} != {1:X6}", key, pair.Key );
                        } else {
                            int diff = pair.Value ^ pair.Key;
                            map.Add( (char)pair.Key, diff );
                        }
                    } else if ( left.LowerCase != pair.Value ) {
                        Console.WriteLine( "{0:X6} != {1:X6}", key, pair.Key );
                    }
                }
            }

            if ( text ) {
                using ( var w = File.CreateText( @"../../bin/OrdinalIgnoreCaseMap.cs" ) ) {
                    map.Write( w );
                }
            } else {
                using(var w=File.Create( @"../../bin/OrdinalIgnoreCaseMap.bin" ))
                using ( var b = new BinaryWriter( w ) ) {
                    map.Save( b );
                }
            }

        }
    }
}
