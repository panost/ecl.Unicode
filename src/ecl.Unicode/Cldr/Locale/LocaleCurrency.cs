using System;
using System.Collections.Generic;
using System.Text;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr.Locale {
    class LocaleCurrency : LdmlAnyNode {
        internal Currency _currency;

        protected override bool HandleAttribute( LdmlAttribute attr, string value ) {
            switch ( attr ) {
            case LdmlAttribute.Type:
                _currency = base.Document.Loader.FindCurrency( value );
                return true;
            }
            return base.HandleAttribute( attr, value );
        }
    }
}
