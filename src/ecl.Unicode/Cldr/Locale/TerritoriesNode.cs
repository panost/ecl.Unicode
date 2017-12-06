using System.Collections.Generic;
using ecl.Unicode;
using eclUnicode.Cldr.Doc;

namespace eclUnicode.Cldr.Locale {
    class TerritoriesNode : LdmlNoKeyNode {
        //internal override LdmlNode CreateChildNode( string name ) {
        //    if ( name == "territory" ) {
        //        return new LdmlTypeStringNode();
        //    }
        //    return base.CreateChildNode( name );
        //}
        private Dictionary<Territory, string> _territoryNames;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Territory, string> TerritoryNames {
            get {
                if ( _territoryNames == null ) {
                    _territoryNames = GetTerritoryNames();
                }
                return _territoryNames;
            }
        }
        private Dictionary<Territory, string> GetTerritoryNames() {
            var loader = Document.Loader;
            var all = loader._territories;
            var map = new Dictionary<Territory, string>();
            foreach ( LdmlNode node in Children ) {
                LdmlAnyNode elm = node as LdmlAnyNode;
                if ( elm != null && elm.Name == "territory" ) {
                    Territory ter;
                    if ( !all.TryGetValue( elm.KeyValue, out ter ) ) {
                        loader.Warning( "Unable to find territory " + elm.KeyValue );
                        continue;
                    }
                    if ( elm.AltKey.HasValue() ) {
                        if ( ter.Code[ 0 ] != 'M' || elm.AltKey[ 0 ] != 'v' ) {
                            continue;
                        }
                    }
                    map[ ter ] = elm.Value;
                }
            }
            return map;
        }
    }

    
}
