using System;
using System.Text;

namespace ecl.Unicode.Cldr.Doc {
    class LdmlTypeLengthNode : LdmlNode {
        private FormatLength _length;
        /// <summary>
        /// 
        /// </summary>
        public FormatLength Length {
            get {
                return _length;
            }
        }
        protected override void AppendAttributes( StringBuilder b ) {
            b.AppendFormat( " type='{0}'", _length.ToCode() );
        }
        protected override bool HandleAttribute( LdmlAttribute attr, string value ) {
            if ( !base.HandleAttribute( attr, value ) ) {
                if ( attr == LdmlAttribute.Type ) {
                    if ( !Enum.TryParse( value, true, out _length ) ) {
                        throw new FormatException( "Invalid length unit " + value );
                    }
                }
            }
            return true;
        }

        protected virtual bool SameValue( string value ) {
            FormatLength t;
            if ( Enum.TryParse( value, true, out t ) ) {
                return t == _length;
            }
            return false;
        }

        public override bool SameNode( LdmlNode other ) {
            LdmlTypeLengthNode nd = other as LdmlTypeLengthNode;
            return nd != null
                   && nd.Name == Name
                   && nd._length == _length;
        }

        internal override bool HasAttributes( LdmlAttributeValue[] filter ) {
            if ( filter == null )
                return true;
            switch ( filter.Length ) {
            case 0:
                return true;
            case 1:
                return filter[ 0 ].Name == LdmlAttribute.Type
                       && SameValue( filter[ 0 ].Value );
            }
            return false;
        }

        public override string GetAttribute( LdmlAttribute attr ) {
            if ( attr == LdmlAttribute.Type ) {
                return _length.ToCode();
            }
            return null;
        }
    }
}
