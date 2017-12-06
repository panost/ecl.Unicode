using System;
using System.Collections.Generic;
using ecl.Unicode;

namespace eclUnicode.Ucd {
    partial class UcdLoader {
        class AliasSection : NamedObject {
        }
        // http://www.unicode.org/reports/tr44/#Property_And_Value_Aliases
        public void LoadAliases() {
            List<string> segs = new List<string>();
            Dictionary<string, AliasSection> sections =
                new Dictionary<string, AliasSection>( StringComparer.OrdinalIgnoreCase );
            using ( LineReader reader = OpenLineReader( "PropertyValueAliases.txt" ) ) {
                foreach ( var count in reader.GetLines( segs, 2 ) ) {
                    AliasSection section;
                    string propertyName = segs[ 0 ];
                    if ( !sections.TryGetValue( propertyName, out section ) ) {
                        section = new AliasSection() {
                            Name = propertyName
                        };
                        sections.Add( section.Name, section );
                    }

                }
            }
        }
    }
}
