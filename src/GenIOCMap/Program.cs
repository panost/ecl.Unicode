using ecl.Unicode.Ucd;
using System;
using System.Diagnostics;
using System.Text;
using GenIOCMap.Testing;

namespace GenIOCMap {
    class Program {
        private const string UcdFileName = @"f:\Checkouts\Unicode\Ucd\12.1.0\UCD.zip";

        private static UcdLoader _ucd;
        /// <summary>
        /// 
        /// </summary>
        public static UcdLoader Ucd {
            get {
                if ( _ucd == null ) {
                    //_ucd = new UcdLoader( UcdFileName, UcdLoader.LoadOptions.AllCodes );
                    _ucd = new UcdLoader( UcdFileName, UcdLoader.LoadOptions.Script );
                }
                return _ucd;
            }
        }

        static void DecomposeTest() {
            //Decompose( "ĲώΏ" );
            //Decompose( "ώΏa" );
            Decompose( "A." );
        }

        static void Decompose( string dd ) {
            //var my = ScannerUtil2.Normalize( dd );
            var my = ScannerUtil2.Normalize( dd );
            Debug.WriteLine( $"My: {dd} : '{my}'" );
            var str = dd.Normalize( NormalizationForm.FormKC );
            var all = string.Join( ',', str.ToCharArray() );
            Debug.WriteLine( $"FormKC: {dd} : '{all}' : '{str}' " );
            str = dd.Normalize( NormalizationForm.FormD );
            all = string.Join( ',', str.ToCharArray() );
            Debug.WriteLine( $"FormD: {dd} : '{all}' : '{str}' " );
            str = dd.Normalize( NormalizationForm.FormKD );
            all = string.Join( ',', str.ToCharArray() );
            Debug.WriteLine( $"FormKD: {dd} : '{all}' : '{str}' " );
        }
        static void Main( string[] args ) {
            //ScannerUtil2.TestNormalize();
            //DecomposeTest();
            //Debug.WriteLine( $"Ĳ : {"ĲώΏ".Normalize(NormalizationForm.FormKC).ToCharArray()}" );
            //new TableBuilder( Ucd ).WriteCodes( StringComparison.OrdinalIgnoreCase );
            new ScannerMapBuilder( Ucd ).WriteBreakMap();
            //new ScannerMapBuilder( Ucd ).WriteScriptMap();
            //new ScannerMapBuilder( Ucd ).WriteCodes2();
            //new MapBuild( Ucd ).BuildToUpperMap();
            //new TestCompEq(Ucd).TestCompare();
            //new TestCompEq(Ucd).TestEquals();
            //new HashCodeTest( Ucd ).TestHashCode();
            //new Deseret().Show();
            //OrdinalIgnoreCase.TestMaps();
        }
    }
}
