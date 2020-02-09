using System;
using ecl.Unicode.Ucd;

namespace GenScannerMap {
    class Program {
        private const string UcdFileName = @"f:\Checkouts\Unicode\Ucd\12.1.0\UCD.zip";

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
            new ScannerMapBuilder( Ucd ).WriteCodes();
        }
    }
}
