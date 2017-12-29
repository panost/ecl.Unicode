using System;
using System.Collections.Generic;
using System.Xml;

namespace ecl.Unicode.Cldr.Doc {
    partial class CldrLoader {
        private Dictionary<string, TimeZoneType> _timeZoneInfoMap;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, TimeZoneType>.ValueCollection TimeZones {
            get {
                EnsureTimeZonesLoaded();
                return _timeZoneInfoMap.Values;
            }
        }
        private readonly Dictionary<string, TimeZoneType> _timeZoneGmtMap = new Dictionary<string, TimeZoneType>( StringComparer.OrdinalIgnoreCase );
        Dictionary<string, MetaTimeZone> _metaZones = new Dictionary<string, MetaTimeZone>( StringComparer.OrdinalIgnoreCase );
        Dictionary<string, WindowTimeZone> _windowZones = new Dictionary<string, WindowTimeZone>( StringComparer.OrdinalIgnoreCase );

        public TimeZoneType FindZone(string name) {
            TimeZoneType zone;
            EnsureTimeZonesLoaded();
            if( !_timeZoneInfoMap.TryGetValue( name, out zone ) ) {
                const string GmtPrefix = "Etc/GMT";
                if ( name.StartsWith( GmtPrefix, StringComparison.OrdinalIgnoreCase ) ) {
                    zone = _timeZoneGmtMap.GetOrCreate( name );
                }
            }
            return zone;
        }

        private void EnsureTimeZonesLoaded() {
            if ( _timeZoneInfoMap == null ) {
                LoadAllTimeZones();
            }
        }

        private void LoadAllTimeZones() {
            if ( _timeZoneInfoMap != null ) {
                return;
            }
            _timeZoneInfoMap = new Dictionary<string, TimeZoneType>( StringComparer.OrdinalIgnoreCase );
            TimeZoneLoader loader = new TimeZoneLoader( this );
            using( XmlReader reader = OpenXmlFile( "supplemental", "metaZones" ) ) {
                reader.MoveToContent();
                reader.LoadElements( loader.ReadMetaZonesRoot );
            }
            using( XmlReader reader = OpenXmlFile( "supplemental", "windowsZones" ) ) {
                reader.MoveToContent();
                reader.LoadElements( loader.ReadWindowsZones );
            }
        }


        public MetaTimeZone GetMetaZone( string name ) {
            EnsureTimeZonesLoaded();
            return _metaZones.GetOrCreate( name );
        }
        public WindowTimeZone FindWindowTimeZone( string name ) {
            EnsureTimeZonesLoaded();
            // "Mid-Atlantic Standard Time" => "Etc/GMT+2"
            // "E. Europe Standard Time" => "GTB Standard Time"
            // "North Korea Standard Time" => Asia/Pyongyang
            // "Kamchatka Standard Time"
            return _windowZones.GetValueOrDefault( name );
        }

        internal class TimeZoneLoader {
            private CldrLoader _loader;
            public TimeZoneLoader( CldrLoader loader) {
                _loader = loader;
            }

            public bool ReadMetaZonesChildren( XmlReader reader ) {
                switch( reader.Name ) {
                case "metazoneInfo":
                    reader.LoadElements( LoadInnerMetaZones );
                    break;
                case "mapTimezones":
                    reader.LoadNodes( LoadMapZone );
                    break;
                default:
                    return false;
                }
                return true;
            }
            public bool ReadMetaZonesRoot( XmlReader reader ) {
                switch( reader.Name ) {
                case "metaZones":
                    reader.LoadElements( ReadMetaZonesChildren );
                    break;
                //case "primaryZones":
                //    reader.LoadNodes( LoadPrimaryZones );
                //    break;
                default:
                    return false;
                }
                return true;
            }

            private void LoadMapZone( string elmName, List<AttributeValue> attributes ) {
                if ( elmName != "mapZone" ) {
                    return;
                }
                MetaTimeZone meta = null;
                Territory territory = null;
                TimeZoneType zone = null;
                foreach ( AttributeValue attribute in attributes ) {
                    switch ( attribute.Name ) {
                    case "other":
                        meta = _loader.GetMetaZone( attribute.Value );
                        break;
                    case "territory":
                        territory = _loader.GetTerritory( attribute.Value );
                        break;
                    case "type":
                        zone = _loader._timeZoneInfoMap[ attribute.Value ];
                        break;
                    }
                }
                if ( meta != null && zone != null ) {
                    if ( territory == null || territory.Code.SameName( "GMT" ) ) {
                        meta.Default = zone;
                    } else {
                        meta.Add( territory, zone );
                    }
                }
            }

            private bool LoadInnerMetaZones( XmlReader reader ) {
                string type;
                if ( reader.Name != "timezone" 
                    || !( type = reader.GetAttribute( "type" ) ).HasValue() ) {
                    return false;
                }
                TimeZoneType info;
                if ( !_loader._timeZoneInfoMap.TryGetValue( type, out info ) ) {
                    info = new TimeZoneType();
                    info.Name = type;
                    _loader._timeZoneInfoMap.Add( type, info );
                } else {
                    throw new Exception( $"Duplicate timezone {type}" );
                }
                reader.LoadElements( _loader, info.LoadUses );

                return true;
            }

            public bool ReadWindowsZones( XmlReader reader ) {
                if( reader.Name != "windowsZones" ) {
                    return false;
                }
                reader.LoadElements( LoadWindowMaps );
                return true;
            }

            private bool LoadWindowMaps( XmlReader reader ) {
                if( reader.Name != "mapTimezones" ) {
                    return false;
                }
                reader.LoadNodes( LoadWindowMap );
                return true;
            }

            private void LoadWindowMap( string name, List<AttributeValue> arg2 ) {
                if( name != "mapZone" ) {
                    return;
                }
                Territory territory = null;
                WindowTimeZone zone = null;
                string[] types = null;
                foreach ( var a in arg2 ) {
                    switch ( a.Name ) {
                    case "other":
                        zone = _loader._windowZones.GetOrCreate( a.Value );
                        break;
                    case "territory":
                        territory = _loader.GetTerritory( a.Value );
                        break;
                    case "type":
                        types = a.Value.SplitAtSpaces();
                        
                        break;
                    }
                }
                if ( zone != null && types != null && types.Length > 0 ) {

                    if ( territory == null || territory.Code == "001" ) {
                        if ( types.Length == 1 ) {
                            zone.Default = _loader.FindZone( types[ 0 ] );
                            return;
                        }
                    }
                    
                    var zones = new TimeZoneType[ types.Length ];
                    for( int i = 0; i < types.Length; i++ ) {
                        TimeZoneType z = _loader.FindZone( types[ i ] );
                        zones[ i ] = z;
                    }
                    zone.Add( territory, zones );
                }
            }
        }
    }
}
