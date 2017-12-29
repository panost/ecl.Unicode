using System;
using System.Text;
using System.Xml;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr.Locale {
    partial class CldrLocale {
        public static readonly CldrLocale[] EmptyArray = new CldrLocale[ 0 ];

        
        private CldrLocale[] _children;
        /// <summary>
        /// 
        /// </summary>
        public CldrLocale[] Children {
            get {
                return _children;
            }
            internal set {
                _children = value;
            }
        }
        internal LanguageInfo _info;


        private CldrLocale _localeParent;
        /// <summary>
        /// 
        /// </summary>
        public CldrLocale LocaleParent {
            get {
                if( _localeParent == null && Name.IndexOf( '_' ) > 0 ) {
                    _localeParent = Loader.GetLocaleParent( Name );
                }
                return _localeParent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WritingScript DefaultScript {
            get {
                if ( _script != 0 ) {
                    return _script;
                }
                if( _info.Scripts != null ) {
                    return _info.Scripts[ 0 ];
                }
                if ( LocaleParent != null ) {
                    return LocaleParent.DefaultScript;
                }
                return WritingScript.Unknown;
                
            }
        }


        public new CldrLocale Parent {
            get {
                return (CldrLocale)base.Parent;
            }
        }


        internal CldrLocale( CldrLoader loader, CldrLocale parent, LanguageInfo info )
            : base( loader, parent, info.Code ) {
            _info = info;
            info.Locale = this;
        }

        private sbyte _nameLevel;
        /// <summary>
        /// 
        /// </summary>
        public sbyte NameLevel {
            get {
                return _nameLevel;
            }
            //set {
            //    _nameLevel = value;
            //}
        }

        private DateTime _generation;
        /// <summary>
        /// 
        /// </summary>
        public DateTime Generation {
            get {
                return _generation;
            }
        }
        private int _revision;
        /// <summary>
        /// 
        /// </summary>
        public int Revision {
            get {
                return _revision;
            }
            set {
                _revision = value;
            }
        }

        private Territory _territory;
        public Territory Territory {
            get {
                return _territory;
            }
        }
        private string _territoryCode;
        /// <summary>
        /// 
        /// </summary>
        public string TerritoryCode {
            get {
                return _territory!=null?_territory.Code:_territoryCode;
            }
        }
        private string _language;
        /// <summary>
        /// 
        /// </summary>
        public string Language {
            get {
                return _language;
            }
            set {
                _language = value;
            }
        }

        private WritingScript _script;
        /// <summary>
        /// 
        /// </summary>
        public WritingScript Script {
            get {
                return _script;
            }
            set {
                _script = value;
            }
        }
        private string _variant;

        /// <summary>
        /// 
        /// </summary>
        public string Variant {
            get {
                return _variant;
            }
            set {
                _variant = value;
            }
        }
        private string _description;
        /// <summary>
        /// 
        /// </summary>
        public string Desciption {
            get {
                return _description;
            }
            set {
                _description = value;
            }
        }

        protected override void AppendString( StringBuilder b ) {
            if ( Parent == null ) {
                b.Append( Name );
                return;
            }
            if ( _language.HasValue() ) {
                b.Append( _language );
            }
            if ( _script != 0 ) {
                b.Append( " script:" );
                b.Append( _script );
            }
            if ( _territory != null ) {
                b.Append( " territory:" );
                b.Append( _territory.Description ?? _territory.Code );
            } else if ( _territoryCode.HasValue() ) {
                b.Append( " territory:" );
                b.Append( _territoryCode );
            }
            if ( _variant.HasValue() ) {
                b.Append( " variant:" );
                b.Append( _variant );
            }
            b.Append( " path:" );
            AppendPath( b );
        }

        internal void ReadIdentity( XmlReader reader ) {
            while ( reader.Read() ) {
                switch ( reader.NodeType ) {
                case XmlNodeType.EndElement:
                    if ( reader.Name == "identity" ) {
                        return;
                    }
                    break;
                case XmlNodeType.Element:
                    var elementName = reader.Name;
                    if ( reader.MoveToFirstAttribute() ) {
                        var attrName = reader.Name;
                        var attrValue = reader.Value;
                        while ( true ) {
                            switch ( elementName ) {
                            case "version":
                                if ( attrName == "number" ) {
                                    _revision = LdmlUtil.ParseRevision( attrValue );
                                }
                                break;
                            case "generation":
                                if ( attrName == "date" ) {
                                    LdmlUtil.TryParseGeneration( attrValue, out _generation );
                                }
                                break;
                            case "language":
                                if ( attrName == "type" ) {
                                    _language = attrValue;
                                }
                                break;
                            case "territory":
                                if ( attrName == "type" ) {
                                    if ( ( _territory = Loader.FindTerritory( attrValue ) ) == null ) {
                                        _territoryCode = attrValue;
                                    }
                                }
                                break;
                            case "script":
                                if ( attrName == "type" ) {
                                    _script = Loader.GetScript( attrValue );
                                }
                                break;
                            case "variant":
                                if ( attrName == "type" ) {
                                    _variant = attrValue;
                                }
                                break;
                            }

                            if ( !reader.MoveToNextAttribute() ) {
                                break;
                            }
                        }
                        reader.MoveToElement();
                    }
                    break;
                }
            }
        }

        internal override LdmlNode CreateNode( string name, LdmlNode parent ) {
            switch ( name ) {
            case "calendar":
                return new CalendarInfo();
            case "unitLength":
                return new LdmlTypeLengthNode();
            case "unit":
                return new LdmlTypeUnitNode();
                
            }
            return new LdmlAnyNode();
        }

        internal void LoadMain( XmlReader reader ) {
            Load( reader );
        }
        internal void LoadMisc( XmlReader reader, string rootName ) {
            reader.MoveToContent();
            if ( reader.IsEmptyElement ) {
                return;
            }
            while ( reader.Read() ) {
                var type = reader.NodeType;
                if ( type == XmlNodeType.Element ) {
                    if ( reader.Name == rootName ) {
                        AddRootNode( ReadRootElement( reader ) );
                    } else {
                        reader.SkipElement();
                    }
                } else if ( type == XmlNodeType.EndElement ) {
                    break;
                }
            }
        }

        internal sealed override LdmlNode ReadRootElement( XmlReader reader ) {
            if ( reader.IsEmptyElement )
                return null;
            string name = Loader.GetCachedName( reader.Name );
            LdmlNode node;
            switch ( name ) {
            case "numbers":
                node = new NumbersInfo();
                break;
            case "localeDisplayNames":
                node = new LocaleDisplayNamesNode();
                break;
            case "characters":
                node = new CharactersNode();
                break;
            case "identity":
                ReadIdentity( reader );
                if ( !string.IsNullOrEmpty( _variant ) ) {
                    _nameLevel = 3;
                } else if ( !string.IsNullOrEmpty( _territoryCode ) ) {
                    _nameLevel = 2;
                } else if ( _script != 0 ) {
                    _nameLevel = 1;
                } else {
                    _nameLevel = 0;
                }
                return null;
            default:
                node = new LdmlNoKeyNode();
                break;
            }
            node.Load( name, this, reader, null );
            return node;
        }
    }
}
