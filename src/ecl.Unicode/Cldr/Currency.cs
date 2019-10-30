using System.Collections.Generic;
using System.Globalization;
using ecl.Unicode.Cldr.Doc;
using ecl.Unicode.Cldr.Locale;

namespace ecl.Unicode.Cldr {
    public class Currency : CodeObjectBase {

        /// <summary>
        /// 
        /// </summary>
        public string Code {
            get => _code;
            set => _code = value;
        }

        private short _id;
        /// <summary>
        /// 
        /// </summary>
        public short Id {
            get => _id;
            set => _id = value;
        }

        internal LocaleCurrency GetLocale( CldrLocale locale ) {
            if ( locale.GetNumberInfo().Currencies.Nodes.TryGetValue( this, out LocaleCurrency e ) ) {
                return e;
            }
            return null;
        }
        public string GetName( CldrLocale locale ) {
            var curr = GetLocale( locale );
            if ( curr != null ) {
                return curr.Select( "displayName" ).GetText();
            }

            return null;
        }
        public string GetSymbol( CldrLocale locale ) {
            var curr = GetLocale( locale );
            if ( curr != null ) {
                return curr.Select( "symbol" ).GetText();
            }

            return null;
        }
        public Currency() {
        }

        internal Currency( List<AttributeValue> list ) {
            foreach ( AttributeValue value in list ) {
                switch ( value.Name ) {
                case "type":
                    _code = value.Value;
                    break;
                case "numeric":
                    short.TryParse( value.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _id );
                    break;
                }
            }
        }

        

        public DateRangeValue<Territory>[] Territories {
            get;
            set;
        }

        /// <summary>
        /// the number of decimal digits to be used when formatting quantities used in cash transactions
        /// </summary>
        public byte CashDigits {
            get;
            set;
        }

        /// <summary>
        /// the rounding increment to be used when formatting quantities used in cash transactions
        /// </summary>
        public byte CashRounding {
            get;
            set;
        }

        /// <summary>
        /// the rounding increment, in units of 10-digits.
        /// The default is 0, which means no rounding is to be done.
        /// </summary>
        public byte Rounding {
            get;
            set;
        }

        /// <summary>
        /// the minimum and maximum number of decimal digits normally formatted
        /// </summary>
        /// <remarks>
        /// http://unicode.org/reports/tr35/tr35-numbers.html#Supplemental_Currency_Data
        /// </remarks>
        public byte Digits {
            get;
            set;
        }

        public bool Deprecated {
            get;
            set;
        }
    }
}
