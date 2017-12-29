using System.Diagnostics;

namespace ecl.Unicode.Cldr {
    [DebuggerDisplay( "Calendar {Name}" )]
    public class Calendar {
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }
    }
}
