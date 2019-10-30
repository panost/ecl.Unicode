using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr.Locale {
    public class NumberSymbolsNode : LdmlAnyNode {

        /// <summary>
        /// 
        /// </summary>
        public string Decimal => this.Select( "decimal" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string Group => this.Select( "group" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string List => this.Select( "list" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string PercentSign => this.Select( "percentSign" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string PlusSign => this.Select( "plusSign" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string MinusSign => this.Select( "minusSign" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string Exponential => this.Select( "exponential" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string SuperscriptingExponent => this.Select( "superscriptingExponent" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string PerMille => this.Select( "perMille" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string Infinity => this.Select( "infinity" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string Nan => this.Select( "nan" ).GetText();

        /// <summary>
        /// 
        /// </summary>
        public string TimeSeparator => this.Select( "timeSeparator" ).GetText();
    }
}
