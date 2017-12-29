using System;

namespace ecl.Unicode.Cldr.Doc {
    [AttributeUsage( AttributeTargets.Field )]
    class KeyCodeAttribute : Attribute {
        public readonly string Code;

        public KeyCodeAttribute( string code ) {
            Code = code;
        }
    }
}
