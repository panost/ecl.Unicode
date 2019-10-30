using System.Text;

namespace ecl.Unicode.Cldr.Doc {
    public class LdmlAnyNode : LdmlNode {
        private LdmlAttribute _keyType;
        /// <summary>
        /// 
        /// </summary>
        public LdmlAttribute KeyType {
            get => _keyType;
            set => _keyType = value;
        }

        private LdmlAttribute _altKeyType;
        /// <summary>
        /// 
        /// </summary>
        public LdmlAttribute AltKeyType {
            get => _altKeyType;
            set => _altKeyType = value;
        }

        private string _keyValue;
        /// <summary>
        /// 
        /// </summary>
        internal string KeyValue {
            get => _keyValue;
            set => _keyValue = value;
        }

        private string _altKey;
        /// <summary>
        /// 
        /// </summary>
        public string AltKey {
            get => _altKey;
            set => _altKey = value;
        }
        protected override bool HandleAttribute( LdmlAttribute attr, string value ) {
            if ( base.HandleAttribute( attr, value ) )
                return true;
            if ( _keyType == 0 ) {
                _keyType = attr;
                _keyValue = value;
                return true;
            }
            if ( _altKeyType == 0 ) {
                _altKeyType = attr;
                _altKey = value;
                return true;
            }
            return false;
        }

        protected override void AppendAttributes( StringBuilder b ) {
            if ( _keyType != 0 ) {
                b.AppendFormat( " {0}='{1}'", _keyType, _keyValue );
            }
            if ( _altKeyType != 0 ) {
                b.AppendFormat( " {0}='{1}'", _altKeyType, _altKey );
            }
            base.AppendAttributes( b );
        }

        protected bool SameAnyNode( LdmlAnyNode other ) {
            if ( other != null
                && other._keyType == _keyType
                 && other._keyValue == _keyValue
                 && other.Name == Name
                 && other._altKeyType == _altKeyType
                 && other._altKey == _altKey
                ) {
                return LdmlUtil.EqualAttributes( Attributes, other.Attributes );
            }
            return false;
        }
        public override string GetAttribute( LdmlAttribute attr ) {
            if ( _keyType == attr ) {
                return _keyValue;
            }
            if ( _altKeyType == attr ) {
                return _altKey;
            }
            return Attributes.GetValue( attr );
        }

        public override bool SameNode( LdmlNode other ) {
            return SameAnyNode( other as LdmlAnyNode );
        }
        public override int GetHashCode() {
            int hc = base.GetHashCode();
            if ( _keyType != 0 ) {
                hc += hc ^ _keyType.GetHashCode() ^ _keyValue.GetHashCode();
            }
            if ( _altKeyType != 0 ) {
                hc += hc ^ _altKeyType.GetHashCode() ^ _altKey.GetHashCode();
            }
            return hc;
        }
    }
}
