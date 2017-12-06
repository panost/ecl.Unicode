using System.Diagnostics;

namespace eclUnicode.Cldr {
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
