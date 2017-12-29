using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr.Locale {
    public class NumberSymbolsNode : LdmlAnyNode {

        /// <summary>
        /// 
        /// </summary>
        public string Decimal {
            get {
                return this.Select( "decimal" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Group {
            get {
                return this.Select( "group" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string List {
            get {
                return this.Select( "list" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PercentSign {
            get {
                return this.Select( "percentSign" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PlusSign {
            get {
                return this.Select( "plusSign" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MinusSign {
            get {
                return this.Select( "minusSign" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Exponential {
            get {
                return this.Select( "exponential" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SuperscriptingExponent {
            get {
                return this.Select( "superscriptingExponent" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PerMille {
            get {
                return this.Select( "perMille" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Infinity {
            get {
                return this.Select( "infinity" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Nan {
            get {
                return this.Select( "nan" ).GetText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TimeSeparator {
            get {
                return this.Select( "timeSeparator" ).GetText();
            }
        }


        
    }
}
