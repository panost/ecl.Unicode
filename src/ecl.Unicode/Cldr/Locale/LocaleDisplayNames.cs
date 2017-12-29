using System.Collections.Generic;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr.Locale {
    public class LocaleDisplayNamesNode : LdmlNoKeyNode {
        internal override LdmlNode CreateChildNode( string name ) {
            switch ( name ) {
            case "territories":
                return new TerritoriesNode();
            case "languages":
                return new DictionayOwnerNode( "language" );
            }
            return base.CreateChildNode( name );
        }

        LdmlNode LocaleDisplayPattern {
            get {
                return this.Select( "localeDisplayPattern" );
            }
        }
        public string LocalePattern {
            get {
                return LocaleDisplayPattern.Select( "localePattern" ).GetText();
            }
        }
        public string LocaleSeparator {
            get {
                return LocaleDisplayPattern.Select( "localeSeparator" ).GetText();
            }
        }
        public string LocaleKeyTypePattern {
            get {
                return LocaleDisplayPattern.Select( "localeKeyTypePattern" ).GetText();
            }
        }
        public Dictionary<Territory, string> TerritoryNames {
            get {
                TerritoriesNode node = this.Select( "territories" ) as TerritoriesNode;
                if ( node != null ) {
                    return node.TerritoryNames;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> LanguageNames {
            get {
                DictionayOwnerNode node = this.Select( "languages" ) as DictionayOwnerNode;
                if ( node != null ) {
                    return node.Map;
                }
                return null;
            }
        }
    }
}
