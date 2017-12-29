using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr {
    public class TimeZoneType : NamedObject {
        private List<DateRangeValue<MetaTimeZone>> _uses = new List<DateRangeValue<MetaTimeZone>>();
        /// <summary>
        /// 
        /// </summary>
        public List<DateRangeValue<MetaTimeZone>> Uses {
            get { return _uses; }
        }

        public TimeZoneType() {
        }
        private static DateTime ParseDate( string val ) {
            DateTime dt;
            if( DateTime.TryParseExact( val, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out dt ) ) {
                return dt;
            }
            throw new FormatException( val );
        }

        internal bool LoadUses( XmlReader reader, CldrLoader loader ) {
            if( reader.Name == "usesMetazone" ) {
                int count = reader.AttributeCount;
                var uses = new DateRangeValue<MetaTimeZone>();

                for( int i = 0; i < count; i++ ) {
                    reader.MoveToAttribute( i );
                    var val = reader.Value;
                    if( !val.HasValue() )
                        continue;
                    switch( reader.Name ) {
                    case "from":
                        uses.From = ParseDate( val );
                        break;
                    case "to":
                        uses.To = ParseDate( val );
                        break;
                    case "mzone":
                        uses.Value = loader.GetMetaZone( val );
                        break;
                    }
                }
                reader.MoveToElement();
                if( uses.Value != null ) {
                    _uses.Add( uses );
                }
            }
            return false;
        }
    }
}
