using System;
using System.Globalization;
using eclUnicode.Cldr.Doc;
using System.Collections.Generic;
using ecl.Unicode;

namespace eclUnicode.Cldr.Locale {
    public class NumberFormatsNode : LdmlAnyNode {
        public readonly NumberType Type;

        internal NumberFormatsNode( NumberType numberType ) {
            Type = numberType;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FormatPattern {
            get {
                return this.Select(
                    new NodePathEntry( Type.ToFormatLength() /*"decimalFormatLength"*/ ),
                    new NodePathEntry( Type.ToFormatName() /*"decimalFormat"*/ ),
                    new NodePathEntry( "pattern" )
                ).GetText();
            }
        }

        public class Pattern : LdmlAnyNode {
            private long _types;
            /// <summary>
            /// 
            /// </summary>
            public long Type {
                get {
                    if ( _types == 0 ) {
                        long res;
                        var val = this.GetAttribute( LdmlAttribute.Type );
                        if ( val.HasValue() && long.TryParse( val, NumberStyles.Integer,
                                 NumberFormatInfo.InvariantInfo, out res ) ) {
                            _types = res;
                        } else {
                            _types = -1;
                        }
                    }
                    return _types;
                }
            }
            private PluralForm _count;
            /// <summary>
            /// 
            /// </summary>
            public PluralForm Count {
                get {
                    if ( _count == 0 ) {
                        var val = this.GetAttribute( LdmlAttribute.Count );
                        if( !val.HasValue() || !Enum.TryParse( val, true, out _count ) ) {
                            _count = (PluralForm)0xFF;
                        }
                    }
                    return _count;
                }
            }

        }

        public class Format : LdmlAnyNode {
            public readonly NumberType Type;

            internal Format( NumberType numberType ) {
                Type = numberType;
            }
            internal override LdmlNode CreateChildNode( string name ) {
                if( name == "pattern" ) {
                    return new Pattern();
                }
                return base.CreateChildNode( name );
            }
            /// <summary>
            /// Get the first pattern
            /// </summary>
            public Pattern Pattern {
                get {
                    foreach ( Pattern pattern in GetPatterns() ) {
                        return pattern;
                    }
                    return null;
                }
            }
            public IEnumerable<Pattern> GetPatterns() {
                return ChildrenOf<Pattern>();
            }
        }
        public class FormatLength : LdmlAnyNode {
            public readonly NumberType Type;

            internal FormatLength( NumberType numberType ) {
                Type = numberType;
            }
            internal override LdmlNode CreateChildNode( string name ) {
                if( name == Type.ToFormatName() ) {
                    return new Format( Type );
                }
                return base.CreateChildNode( name );
            }
            public IEnumerable<Format> GetFormats() {
                return ChildrenOf<Format>();
            }
            private eclUnicode.Cldr.FormatLength _type;
            /// <summary>
            /// 
            /// </summary>
            public eclUnicode.Cldr.FormatLength Length {
                get {
                    if( _type == 0 ) {
                        var val = this.GetAttribute( LdmlAttribute.Count );
                        if( !val.HasValue() || !Enum.TryParse( val, true, out _type ) ) {
                            _type = eclUnicode.Cldr.FormatLength.Narrow;
                        }
                    }
                    return _type;
                }
            }
        }

        internal override LdmlNode CreateChildNode( string name ) {
            if ( name == Type.ToFormatLength() ) {
                return new FormatLength( Type );
            }
            return base.CreateChildNode( name );
        }

        public IEnumerable<FormatLength> GetFormatLengths() {
            return ChildrenOf<FormatLength>();
        }

        public FormatLength GetFormatLength( eclUnicode.Cldr.FormatLength length = eclUnicode.Cldr.FormatLength.Narrow ) {
            switch ( length ) {
            case eclUnicode.Cldr.FormatLength.Narrow:
            case eclUnicode.Cldr.FormatLength.Long:
            case eclUnicode.Cldr.FormatLength.Short:
                foreach( var flen in GetFormatLengths() ) {
                    if( flen.Length == length ) {
                        return flen;
                    }
                }
                break;
            }
            return null;
        }
    }
}
