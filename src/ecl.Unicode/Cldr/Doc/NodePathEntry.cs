namespace eclUnicode.Cldr.Doc {
    struct NodePathEntry {
        public string Name;
        public LdmlAttributeValue[] Attributes;

        public NodePathEntry( string name, LdmlAttributeValue[] nodeAttribute = null ) {
            Name = name;
            Attributes = nodeAttribute ?? LdmlAttributeValue.EmptyArray;
        }
        public NodePathEntry( string name, string attrValue, LdmlAttribute attrType = LdmlAttribute.Type ) {
            Name = name;
            Attributes = new[] { new LdmlAttributeValue( attrType, attrValue ) };
        }
        public NodePathEntry( string name, LdmlAttributeValue nodeAttribute ) {
            Name = name;
            Attributes = new[] { nodeAttribute };
        }
    }
}
