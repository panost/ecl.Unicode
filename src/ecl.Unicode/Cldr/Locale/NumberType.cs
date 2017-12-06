
using ecl.Unicode;

namespace eclUnicode.Cldr.Locale {
    public enum NumberType : byte {
        Unknown,
        Currency,
        Decimal,
        Percent,
        Scientific,
        Accounting
    }
}

namespace eclUnicode.Cldr {
    using System;
    using eclUnicode.Cldr.Locale;

    public static partial class CldrUtil {
        private static readonly string[] _numberTypes = {
            "currency", "decimal", "percent", "scientific", "accounting"
        };

        public static string ToCode( this NumberType width ) {
            if( width > 0 && width <= NumberType.Accounting ) {
                return _numberTypes[ (int)width - 1 ];
            }
            return null;
        }
        private static readonly string[] _numberFormatNodeName = {
            "currencyFormats", "decimalFormats", "percentFormats", "scientificFormats"
        };
        /// <summary>
        /// currencyFormats, decimalFormats, ..
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string NodeName( this NumberType width ) {
            if( width > 0 && width <= NumberType.Scientific ) {
                return _numberFormatNodeName[ (int)width - 1 ];
            }
            return null;
        }
        private static string[] _numberFormatLength;
        /// <summary>
        /// currencyFormatLength, decimalFormatLength, ...
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string ToFormatLength( this NumberType width ) {
            if( width > 0 && width <= NumberType.Scientific ) {
                if( _numberFormatLength == null ) {
                    _numberFormatLength = new string[4];
                    for ( int i = 0; i < 4; i++ ) {
                        _numberFormatLength[ i ] = _numberTypes[ i ] + "FormatLength";
                    }
                }
                return _numberFormatLength[ (int)width - 1 ];
            }
            return null;
        }
        private static string[] _numberFormatName;
        /// <summary>
        /// currencyFormat,decimalFormat,...
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string ToFormatName( this NumberType width ) {
            if( width > 0 && width <= NumberType.Scientific ) {
                if( _numberFormatName == null ) {
                    _numberFormatName = new string[ 4 ];
                    for( int i = 0; i < 4; i++ ) {
                        _numberFormatName[ i ] = _numberTypes[ i ] + "Format";
                    }
                }
                return _numberFormatName[ (int)width - 1 ];
            }
            return null;
        }

        
        public static NumberType GetNumberType( string name ) {
            if( name.HasValue() ) {
                var idx = name.IndexOf( "Format", StringComparison.Ordinal );
                if( idx >= 7 ) {
                    name = name.Substring( 0, idx );
                }
                idx = Array.IndexOf( _numberTypes, name );
                if( idx >= 0 ) {
                    return (NumberType)( idx + 1 );
                }
            }
            return 0;
        }
    }
}