using System;
using System.Collections.Generic;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr {
    public class FormatSizeEntry {
        private string _full;
        /// <summary>
        /// 
        /// </summary>
        public string Full {
            get {
                return _full;
            }
            set {
                _full = value;
            }
        }

        private string _long;
        /// <summary>
        /// 
        /// </summary>
        public string Long {
            get {
                return _long;
            }
            set {
                _long = value;
            }
        }

        private string _medium;
        /// <summary>
        /// 
        /// </summary>
        public string Medium {
            get {
                return _medium;
            }
            set {
                _medium = value;
            }
        }

        private string _short;
        /// <summary>
        /// 
        /// </summary>
        public string Short {
            get {
                return _short;
            }
            set {
                _short = value;
            }
        }
        private string _narrow;
        /// <summary>
        /// 
        /// </summary>
        public string Narrow {
            get {
                return _narrow;
            }
            set {
                _narrow = value;
            }
        }

        public string this[ FormatLength size ] {
            get {
                switch ( size ) {
                case FormatLength.Full:
                    return _full;
                case FormatLength.Long:
                    return _long;
                case FormatLength.Short:
                    return _short;
                case FormatLength.Narrow:
                    return _narrow;
                case FormatLength.Medium:
                    return _medium;
                }
                return null;
            }
            internal set {
                switch ( size ) {
                case FormatLength.Full:
                    _full = value;
                    break;
                case FormatLength.Long:
                    _long = value;
                    break;
                case FormatLength.Short:
                    _short = value;
                    break;
                case FormatLength.Narrow:
                    _narrow = value;
                    break;
                case FormatLength.Medium:
                    _medium = value;
                    break;
                }
            }
        }
        public string[] UniqueEntries() {
            List<string> list=new List<string>(5);
            if ( _full.HasValue() ) {
                list.Add( _full );
            }
            if ( _long.HasValue() && list.IndexOf( _long ) < 0 ) {
                list.Add( _long );
            }
            if ( _medium.HasValue() && list.IndexOf( _medium ) < 0 ) {
                list.Add( _medium );
            }
            if ( _short.HasValue() && list.IndexOf( _short ) < 0 ) {
                list.Add( _short );
            }
            if ( _narrow.HasValue() && list.IndexOf( _narrow ) < 0 ) {
                list.Add( _narrow );
            }
            return list.ToArray();
        }
        internal void LoadEntries( LdmlNode root, string typeNodeName, params string[] path ) {
            if ( root != null ) {
                foreach ( LdmlAnyNode node in root.Children ) {
                    FormatLength size;
                    if ( node.Name != typeNodeName ||
                        !Enum.TryParse( node.KeyValue, true, out size ) ) {
                        continue;
                    }
                    var val = node.Select( path ).GetText();
                    if ( val.HasValue() ) {
                        this[ size ] = val;
                    }
                }
            }
        }
        
    }
}
