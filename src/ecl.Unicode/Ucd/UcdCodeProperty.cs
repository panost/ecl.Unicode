using System.Collections.Generic;

namespace ecl.Unicode.Ucd {
    public class UcdCodeProperty : CodeObjectBase {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name => _code;

        private CodePointProperty _value;
        /// <summary>
        /// 
        /// </summary>
        public CodePointProperty Value => _value;

        private bool _isDerived;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDerived {
            get => _isDerived;
            set => _isDerived = value;
        }

        public UcdCodeProperty( string name, CodePointProperty value ) {
            _code = name;
            _value = value;
        }

        private readonly List<UcdRange<UcdCodeProperty>> _ranges = new List<UcdRange<UcdCodeProperty>>();
        /// <summary>
        /// 
        /// </summary>
        public List<UcdRange<UcdCodeProperty>> Ranges => _ranges;

        public bool Contains( int code ) {
            foreach ( UcdRange<UcdCodeProperty> range in _ranges ) {
                if ( range.Contains( code ) ) {
                    return true;
                }
            }

            return false;
        }



    }
}
