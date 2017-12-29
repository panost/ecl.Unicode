using System.Collections.Generic;

namespace ecl.Unicode.Cldr {
    public class MetaTimeZone : NamedObject {
        private TimeZoneType _default;
        /// <summary>
        /// 
        /// </summary>
        public TimeZoneType Default {
            get { return _default; }
            set { _default = value; }
        }

        private Dictionary<Territory, TimeZoneType> _territories;
        public void Add( Territory territory, TimeZoneType zone ) {
            if( _territories == null ) {
                _territories = new Dictionary<Territory, TimeZoneType>();
            }
            _territories.Add( territory, zone );
        }
    }
}
