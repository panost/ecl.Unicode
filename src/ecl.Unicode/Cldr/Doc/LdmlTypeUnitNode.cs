using System;
using System.Text;
using ecl.Unicode;

namespace eclUnicode.Cldr.Doc {
    class LdmlTypeUnitNode : LdmlNode {
        private CldrUnit _unit;
        /// <summary>
        /// 
        /// </summary>
        public CldrUnit Unit {
            get {
                return _unit;
            }
        }
        protected override void AppendAttributes( StringBuilder b ) {
            b.AppendFormat( " type='{0}'", _unit.Name );
        }
        protected override bool HandleAttribute( LdmlAttribute attr, string value ) {
            if ( !base.HandleAttribute( attr, value ) ) {
                if ( attr == LdmlAttribute.Type ) {
                    _unit = Document.GetOrCreateUnit( value );
                }
            }
            return true;
        }

        protected virtual bool SameValue( string value ) {
            return _unit != null
                   && _unit.Name.SameName( value );
        }

        public override bool SameNode( LdmlNode other ) {
            LdmlTypeUnitNode nd = other as LdmlTypeUnitNode;
            return nd != null
                   && nd.Name == Name
                   && nd._unit == _unit;
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
                return _unit.Name;
            }
            return null;
        }
    }
}
