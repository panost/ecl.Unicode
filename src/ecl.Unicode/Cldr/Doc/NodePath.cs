using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ecl.Unicode.Cldr.Doc {
    internal class NodePath {
        private static readonly char[] _pathDelims = { '/', '[' };

        public readonly int Level;
        private NodePathEntry[] _entries;
        public readonly string Path;

        //private NodePath( int level, Entry[] entries ) {
        //    Level = level;
        //    _entries = entries;
        //}


        

        public static LdmlAttributeValue[] ParseAttrs( string attrText, List<LdmlAttributeValue> attrList ) {
            int idx = attrText.IndexOf( '=' );
            if ( idx > 0 ) {
                string name = attrText.Substring( 1, idx - 1 );
                LdmlAttribute attrName = LdmlUtil.GetAttribute( name );
                string value = attrText.Substring( idx + 1 );
                if ( value.Length > 2 ) {
                    if ( value[ 0 ] == '\'' && value[ value.Length - 1 ] == '\'' ) {
                        value = value.Substring( 1, value.Length - 2 );
                        attrList.Add( new LdmlAttributeValue( attrName, value ) );
                    } else {
                        Debug.WriteLine( value );
                    }
                }
            }
            return attrList.ToArray();
        }

        public NodePath( string path ) {
            Path = path;
            while ( path.StartsWith( "../", StringComparison.Ordinal ) ) {
                Level++;
                path = path.Substring( 3 );
            }
            List<NodePathEntry> list = new List<NodePathEntry>();
            List<LdmlAttributeValue> attrList = new List<LdmlAttributeValue>();

            int idx = path.IndexOfAny( _pathDelims );
            while ( idx > 0 ) {
                var delim = path[ idx ];
                var elmName = path.Substring( 0, idx );
                if ( delim == '/' ) {
                    // empty elm
                    path = path.Substring( idx + 1 );
                    list.Add( new NodePathEntry( elmName ) );
                } else if ( delim == '[' ) {
                    attrList.Clear();
                    int idx2 = path.IndexOf( ']', idx + 1 );
                    string attr = path.Substring( idx + 1, idx2 - idx - 1 );
                    ParseAttrs( attr, attrList );
                    idx2++;
                    if ( idx2 < path.Length ) {
                        delim = path[ idx2 ];
                        if ( delim == '/' ) {
                            idx2++;
                        }
                        path = path.Substring( idx2 );
                    } else {
                        path = "";
                    }

                    list.Add( new NodePathEntry( elmName, attrList.ToArray() ) );

                }
                idx = path.IndexOfAny( _pathDelims );
            }
            if ( path.HasValue() ) {
                list.Add( new NodePathEntry( path ) );
            }
            _entries = list.ToArray();
            //return new NodePath( level, list.ToArray() );
        }

        internal LdmlNode SelectNode( LdmlDocument document ) {
            if ( Level != 0 || _entries.Length == 0 || _entries[ 0 ].Attributes.Length != 0 )
                return null;
            LdmlNode root = document.ResolveRootNode( _entries[ 0 ].Name );
            for ( int i = 1; root != null && i < _entries.Length; i++ ) {
                root = root.SelectNode( _entries[ i ].Name, _entries[ i ].Attributes );
            }
            return root;
        }

        internal LdmlNode ResolveNode( LdmlNode root ) {
            for ( int i = Level; root != null && i > 0; i-- ) {
                root = root.Parent;
            }
            for ( int i = 0; root != null && i < _entries.Length; i++ ) {
                root = root.ResolveNode( _entries[ i ].Name, _entries[ i ].Attributes );
            }
            return root;
        }
    }
}
