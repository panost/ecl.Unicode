using ecl.Unicode;
using eclUnicode.Cldr.Doc;

namespace eclUnicode.Cldr.Locale {
    public class NumbersInfo : LdmlAnyNode {
        private string _numberSystem;

        /// <summary>
        /// 
        /// </summary>
        public string NumberSystem {
            get {
                if ( _numberSystem == null ) {
                    _numberSystem = this.Select( "defaultNumberingSystem" ).GetText();
                }
                return _numberSystem;
            }
        }

        internal override LdmlNode CreateChildNode( string name ) {
            switch ( name ) {
            case "symbols":
                return new NumberSymbolsNode();
            case "currencyFormats":
                return new NumberFormatsNode( NumberType.Currency );
            case "decimalFormats":
                return new NumberFormatsNode( NumberType.Decimal );
            case "percentFormats":
                return new NumberFormatsNode( NumberType.Percent );
            case "scientificFormats":
                return new NumberFormatsNode( NumberType.Scientific );
            case "currencies":
                break;
            }
            return base.CreateChildNode( name );
        }

        internal NumbersInfo() {
        }

        public NumberFormatsNode GetFormats( NumberType type, string numberSystem = null ) {
            var nodeName = type.NodeName();
            if ( nodeName != null ) {
                var node = this.Select(
                    new NodePathEntry( nodeName, numberSystem ?? NumberSystem, LdmlAttribute.NumberSystem ) );

                return (NumberFormatsNode)node;
            }
            return null;
        }
        public NumberFormatsNode.FormatLength GetFormatLength( NumberType type, FormatLength length = FormatLength.Narrow, string numberSystem = null ) {
            return GetFormats( type, numberSystem )?.GetFormatLength( length );
        }
        public NumberFormatsNode.Format GetFormat( NumberType type, FormatLength length = FormatLength.Narrow, string numberSystem = null ) {
            var root = GetFormatLength( type == NumberType.Accounting ? NumberType.Currency : type, length, numberSystem );
            if ( root != null ) {
                foreach ( var fmt in root.GetFormats() ) {
                    var ftype = fmt.GetAttribute( LdmlAttribute.Type );
                    if ( ftype.SameName( "accounting" ) ) {
                        if ( type == NumberType.Accounting ) {
                            return fmt;
                        }
                    } else if ( type != NumberType.Accounting ) {
                        return fmt;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public NumberFormatsNode GetDecimalFormats( string numberSystem = null) {
            return GetFormats( NumberType.Decimal, numberSystem );
        }


        /// <summary>
        /// 
        /// </summary>
        public NumberSymbolsNode Symbols {
            get {
                var node = this.Select(
                    new NodePathEntry( "symbols", NumberSystem, LdmlAttribute.NumberSystem ) );

                return (NumberSymbolsNode)node;
            }
        }
    }
}
