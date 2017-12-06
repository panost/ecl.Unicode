using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using ecl.Unicode;

namespace eclUnicode.Cldr.Doc {
    partial class CldrLoader {
        enum AliasReason {
            Deprecated=1,
            Legacy,
            Macrolanguage,
            Overlong,
            Bibliographic
        }
        struct AliasInfo {
            public string Replacement;
            public AliasReason Reason;

            public bool Many {
                get {
                    return Replacement != null && Replacement.IndexOf( ' ' ) > 0;
                }
            }
        }


        private readonly Dictionary<string, AliasInfo> _languageAliases =
        new Dictionary<string, AliasInfo>( StringComparer.OrdinalIgnoreCase );

        private readonly Dictionary<string, AliasInfo> _territoryAliases =
            new Dictionary<string, AliasInfo>( StringComparer.OrdinalIgnoreCase );

        private readonly Dictionary<string, AliasInfo> _scriptAliases =
            new Dictionary<string, AliasInfo>( StringComparer.OrdinalIgnoreCase );

        partial class MetaDataLoader {

            private readonly CldrLoader _loader;
            public MetaDataLoader( CldrLoader loader ) {
                _loader = loader;
            }

            private void LoadDefaultLocales( string[] names ) {
                if ( names != null ) {
                    foreach ( string name in names ) {
                        string code = name;
                        if ( code != null && ( code = code.Trim() ).HasValue() ) {
                            _loader.GetLocaleInfo( code ).IsDefault = true;
                        }
                    }
                }
            }

            private void LoadAliases( string elmName, List<AttributeValue> attrs ) {
                string code=null;
                AliasInfo info=new AliasInfo();
                Dictionary<string, AliasInfo> aliasMap;
                switch( elmName ) {
                case "scriptAlias":
                    aliasMap = _loader._scriptAliases;
                    break;
                case "languageAlias":
                    aliasMap = _loader._languageAliases;
                    break;
                case "territoryAlias":
                    aliasMap = _loader._territoryAliases;
                    break;
                default:
                    return;
                }
                foreach( AttributeValue attr in attrs ) {
                    switch ( attr.Name ) {
                    case "type":
                        code = attr.Value;
                        break;
                    case "replacement":
                        info.Replacement = attr.Value;
                        break;
                    case "reason":
                        if ( !Enum.TryParse( attr.Value, true, out info.Reason ) ) {
                            info.Replacement = null;
                            //Debug.WriteLine( attr.Value );
                        }
                        break;
                    }
                }
                if ( code.HasValue() && info.Replacement.HasValue() ) {
                    AliasInfo other;
                    if ( aliasMap.TryGetValue( code, out other ) ) {
                        throw new Exception( $"Duplicate alias {code} specified" );
                    }
                    aliasMap.Add( code, info );
                    //if ( code.IndexOf( ' ' ) >= 0 || info.Replacement.IndexOf( ' ' ) >= 0 ) {
                    //    Debug.WriteLine( $"Many replacements {elmName}:'{info.Replacement}'" );
                    //}
                } else {
                    throw new Exception( "Invalid alias specified" );
                }
            }
            private void LoadInnerMetaData( XmlReader reader ) {
                while ( reader.Read() ) {
                    var type = reader.NodeType;
                    if ( type == XmlNodeType.Element ) {
                            switch ( reader.Name ) {
                            case "alias":
                                reader.LoadNodes( LoadAliases );
                                break;
                            case "defaultContent":
                                LoadDefaultLocales( reader.GetAttribute( "locales" )?.SplitAtSpaces() );
                                goto default;
                            //case "elementOrder":
                            //    _currencyRegions.Add( new CurrencyRegion( reader ) );
                            //    break;
                            default:
                                reader.SkipElement();
                                break;
                            }
                    } else if ( type == XmlNodeType.EndElement ) {
                        break;
                    }
                }
            }


            public void LoadMetaData( XmlReader reader ) {
                reader.MoveToContent();
                while ( reader.Read() ) {
                    var type = reader.NodeType;
                    if ( type == XmlNodeType.Element ) {
                        if ( !reader.IsEmptyElement ) {
                            switch ( reader.Name ) {
                            case "metadata":
                                LoadInnerMetaData( reader );
                                break;
                            default:
                                reader.SkipElement();
                                break;
                            }
                        }
                    } else if ( type == XmlNodeType.EndElement ) {
                        break;
                    }
                }
                
            }
        }
    }
}
