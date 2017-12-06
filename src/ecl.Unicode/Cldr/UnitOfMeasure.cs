using System;
using System.Collections.Generic;
using eclUnicode.Cldr.Doc;

namespace eclUnicode.Cldr {
    public class UnitOfMeasure {
        //public readonly LocaleData Locale;
        public readonly string Name;
        public readonly CldrUnit Unit;
        public class UnitSlot {
            public readonly UnitOfMeasure Unit;
            public readonly FormatLength Size;

            public UnitSlot( UnitOfMeasure unit, FormatLength size ) {
                Unit = unit;
                Size = size;
            }
            private string _displayName;
            /// <summary>
            /// 
            /// </summary>
            public string DisplayName {
                get {
                    return _displayName;
                }
                set {
                    _displayName = value;
                }
            }
            private PluralEntry[] _patterns;
            /// <summary>
            /// 
            /// </summary>
            public PluralEntry[] Patterns {
                get {
                    return _patterns;
                }
                //set {
                //    _patterns = value;
                //}
            }
            private string _perUnitPattern;
            /// <summary>
            /// 
            /// </summary>
            public string PerUnitPattern {
                get {
                    return _perUnitPattern;
                }
                set {
                    _perUnitPattern = value;
                }
            }

            internal void Load( LdmlNode root ) {
                var list = new List<PluralEntry>(5);

                foreach ( LdmlAnyNode node in root.Children ) {
                    switch ( node.Name ) {
                    case "displayName":
                        _displayName = node.Value;
                        break;
                    case "perUnitPattern":
                        _perUnitPattern = node.Value;
                        break;
                    case "unitPattern":
                        PluralForm count;
                        if ( Enum.TryParse( node.KeyValue, true, out count ) ) {
                            list.Add( new PluralEntry( count, node.Value ) );
                        }
                        break;
                    }
                }
                _patterns = list.ToArray();
            }
            
            
        }
        private readonly UnitSlot[] _sizes = new UnitSlot[3];

        private static int GetIndex( FormatLength size ) {
            switch ( size ) {
            case FormatLength.Long:
                return 0;
            case FormatLength.Short:
                return 1;
            case FormatLength.Narrow:
                return 2;
            }
            return -1;
        }
        public UnitSlot this[ FormatLength size ] {
            get {
                int idx = GetIndex( size );
                if ( idx >= 0 ) {
                    return _sizes[ idx ];
                }
                return null;
            }
            set {
                int idx = GetIndex( size );
                if ( idx >= 0 ) {
                    _sizes[ idx ] = value;
                }
            }
        }

        internal UnitOfMeasure( CldrUnit unit ) {
            Unit = unit;
            Name = unit.Name;
        }
        //internal UnitOfMeasure( LocaleData locale, string name ) {
        //    WellKnownUnit unit;
        //    if ( XUtil.TryParseUnit( name, out unit ) ) {
        //        Unit = unit;
        //        Name = unit.ToCode();
        //    } else {
        //        Name = name;
        //    }
        //}

        //internal UnitSlot Load( FormatSize size, JObject root ) {
        //    int idx = GetIndex( size );
        //    if ( idx < 0 ) {
        //        return null;
        //    }
        //    UnitSlot slot = new UnitSlot( this, size );
        //    _sizes[ idx ] = slot;
        //    slot.Load( root );
        //    return slot;
        //}

        internal UnitSlot Load( FormatLength size, LdmlTypeUnitNode root ) {
            int idx = GetIndex( size );
            if ( idx < 0 ) {
                return null;
            }
            UnitSlot slot = new UnitSlot( this, size );
            _sizes[ idx ] = slot;
            slot.Load( root );
            return slot;
        }

    }
}
