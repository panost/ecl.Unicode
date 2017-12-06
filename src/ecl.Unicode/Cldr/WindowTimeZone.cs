using System.Collections.Generic;
using ecl.Unicode;

namespace eclUnicode.Cldr {
    public class WindowTimeZone : NamedObject {
        private TimeZoneType _default;
        /// <summary>
        /// 
        /// </summary>
        public TimeZoneType Default {
            get { return _default; }
            set { _default = value; }
        }

        private Dictionary<Territory, TimeZoneType[]> _map = new Dictionary<Territory, TimeZoneType[]>();

        public void Add( Territory territory, TimeZoneType[] zones ) {
            _map.Add( territory, zones );
        }
    }
}
