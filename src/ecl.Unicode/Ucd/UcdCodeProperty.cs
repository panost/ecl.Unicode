using System.Collections.Generic;

namespace ecl.Unicode.Ucd {
    public class UcdCodeProperty : CodeObjectBase {
        /// <summary>
        /// 
        /// </summary>
        public string Name {
            get {
                return _code;
            }
        }
        private CodePointProperty _value;
        /// <summary>
        /// 
        /// </summary>
        public CodePointProperty Value {
            get {
                return _value;
            }
        }

        public UcdCodeProperty( string name, CodePointProperty value ) {
            _code = name;
            _value = value;
        }

        private readonly List<UcdRange<UcdCodeProperty>> _ranges = new List<UcdRange<UcdCodeProperty>>();
        /// <summary>
        /// 
        /// </summary>
        public List<UcdRange<UcdCodeProperty>> Ranges {
            get {
                return _ranges;
            }
        }

        private ulong _mask;
        /// <summary>
        /// 
        /// </summary>
        public ulong Mask {
            get {
                return _mask;
            }
            set {
                _mask = value;
            }
        }

        
    }
}
