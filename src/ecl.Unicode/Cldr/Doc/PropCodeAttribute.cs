using System;

namespace ecl.Unicode.Cldr.Doc {
    [AttributeUsage( AttributeTargets.Field )]
    class PropCodeAttribute : KeyCodeAttribute {
        public readonly bool IsDerived;

        public PropCodeAttribute( string code, bool isDerived = false )
            : base( code ) {
            this.IsDerived = isDerived;
        }
    }
}
