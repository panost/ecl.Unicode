using System.Text;

namespace eclUnicode.Cldr.Doc {
    struct AttributeValue {
        public static readonly AttributeValue[] EmptyArray = new AttributeValue[ 0 ];
        public string Name;
        public string Value;

        public AttributeValue( string name, string value ) {
            Name = name;
            Value = value;
        }


        internal void AppendString( StringBuilder b ) {
            b.AppendFormat( " {0}='{1}'", Name, Value );
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();
            AppendString( b );
            return b.ToString();
        }
    }
}
