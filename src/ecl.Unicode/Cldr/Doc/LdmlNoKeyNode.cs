namespace eclUnicode.Cldr.Doc {
    public class LdmlNoKeyNode : LdmlNode {
        public override bool SameNode( LdmlNode other ) {
            return other != null
                   && other.GetType() == this.GetType()
                   && Name == other.Name;
        }

        protected override bool HandleAttribute( LdmlAttribute attr, string value ) {
            base.HandleAttribute( attr, value );
            return true;
        }

        internal override bool HasAttributes( LdmlAttributeValue[] filter ) {
            return filter == null || filter.Length == 0;
        }

        public override string GetAttribute( LdmlAttribute attr ) {
            return null;
        }
    }
}
