using ecl.Unicode.Ucd;
using System;
using GenIOCMap.Testing;

namespace GenIOCMap {
    class Program {
        private const string UcdFileName = @"../../bin/UCD-10.0.0.zip";

        private static UcdLoader _ucd;
        /// <summary>
        /// 
        /// </summary>
        public static UcdLoader Ucd {
            get {
                if ( _ucd == null ) {
                    _ucd = new UcdLoader( UcdFileName, UcdLoader.LoadOptions.AllCodes );
                }
                return _ucd;
            }
        }
        static void Main( string[] args ) {
            //new TableBuilder( Ucd ).WriteCodes( StringComparison.OrdinalIgnoreCase );
            //new MapBuild( Ucd ).BuildToUpperMap();
            //new TestCompEq(Ucd).TestCompare();
            //new TestCompEq(Ucd).TestEquals();
            //new HashCodeTest( Ucd ).TestHashCode();
            //new Deseret().Show();
            OrdinalIgnoreCase.TestMaps();
        }
    }
}
