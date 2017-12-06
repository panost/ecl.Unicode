using System.Text;

namespace eclUnicode.Cldr.Doc {
    public struct LdmlAttributeValue {
        public static readonly LdmlAttributeValue[] EmptyArray = new LdmlAttributeValue[ 0 ];
        public LdmlAttribute Name;
        public string Value;

        public LdmlAttributeValue( LdmlAttribute name, string value ) {
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
