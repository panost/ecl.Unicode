using System;
using System.Collections.Generic;
using System.Text;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr.Locale {
    public class NumberCurrenciesNode : LdmlAnyNode {
        private Dictionary<Currency, LocaleCurrency> _nodes;
        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<Currency, LocaleCurrency> Nodes {
            get {
                if ( _nodes == null ) {
                    _nodes = CreateMap();
                }

                return _nodes;
            }
        }

        private Dictionary<Currency, LocaleCurrency> CreateMap() {
            var map = new Dictionary<Currency, LocaleCurrency>();
            foreach ( LocaleCurrency child in Children ) {
                if ( child._currency != null ) {
                    map.Add( child._currency, child );
                }
            }

            return map;
        }


        internal override LdmlNode CreateChildNode( string name ) {
            return new LocaleCurrency();
        }
    }
}
