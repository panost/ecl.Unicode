using System.Collections.Generic;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr.Locale {
    class DictionayOwnerNode : LdmlNoKeyNode {
        private readonly string _childName;
        public DictionayOwnerNode( string childName ) {
            _childName = childName;
        }
        private Dictionary<string, string> _map;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Map {
            get {
                if ( _map == null ) {
                    _map = GetLanguageNames();
                }
                return _map;
            }
        }
        private Dictionary<string, string> GetLanguageNames() {
            var map = new Dictionary<string, string>();
            foreach ( LdmlNode node in Children ) {
                LdmlAnyNode elm = node as LdmlAnyNode;
                if ( elm != null && elm.Name == _childName && !elm.AltKey.HasValue() ) {
                    map[ elm.KeyValue ] = elm.Value;
                }
            }
            return map;
        }
    }

    
}
