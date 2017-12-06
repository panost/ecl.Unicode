using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace eclUnicode.Cldr.Doc {
    static class LdmlUtil {
        public static string GetText( this LdmlNode root ) {
            if ( root != null ) {
                return root.Value ?? "";
            }
            return "";
        }
        internal static LdmlNode Select( this LdmlNode root, string name ) {
            if ( root != null ) {
                foreach ( LdmlNode node in root.Children ) {
                    if ( node.Name == name ) {
                        return node;
                    }
                }
            }
            return null;
        }
        internal static LdmlNode Select( this LdmlNode root, string name, LdmlAttributeValue[] filter ) {
            if ( root != null ) {
                foreach ( LdmlNode node in root.Children ) {
                    if ( node.Name == name && node.HasAttributes( filter ) ) {
                        return node;
                    }
                }
            }
            return null;
        }
        internal static LdmlNode Select( this LdmlNode root, string name, string filter, LdmlAttribute tp ) {
            if ( root != null ) {
                foreach ( LdmlNode node in root.Children ) {
                    if ( node.Name == name && node.GetAttribute( tp ) == filter ) {
                        return node;
                    }
                }
            }
            return null;
        }
        internal static LdmlNode Select( this LdmlNode root, string name1, string name2 ) {
            return root.Select( name1 ).Select( name2 );
        }
        internal static LdmlNode Select( this LdmlNode root, params NodePathEntry[] entries ) {
            for ( int i = 0; root != null && i < entries.Length; i++ ) {
                root = root.Select( entries[ i ].Name, entries[ i ].Attributes );
            }
            return root;
        }
        internal static LdmlNode Select( this LdmlNode root, params string[] entries ) {
            for ( int i = 0; root != null && i < entries.Length; i++ ) {
                root = root.Select( entries[ i ] );
            }
            return root;
        }
        internal static LdmlNode SelectNode( this LdmlNode root, string name, LdmlAttributeValue[] filter = null ) {
            if ( root != null ) {
                foreach ( LdmlNode node in root.Children ) {
                    if ( node.Name == name && node.HasAttributes( filter ) ) {
                        return node;
                    }
                }
            }
            return null;
        }
        internal static bool TryParseGeneration( string vv, out DateTime dateTime ) {
            const string DateTag = "$Date:";
            if ( vv.StartsWith( DateTag, StringComparison.OrdinalIgnoreCase )
                 && vv[ vv.Length - 1 ] == '$' ) {
                int idx = vv.IndexOf( '(', DateTag.Length );
                if ( idx > 0 ) {
                    vv = vv.Substring( DateTag.Length, idx - DateTag.Length );
                    return DateTime.TryParseExact( vv, "yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out dateTime );
                    
                }
            }
            dateTime = default(DateTime);
            return false;
        }

        internal static int ParseRevision( string vv ) {
            const string RevTag = "$Revision:";
            if ( vv.StartsWith( RevTag, StringComparison.OrdinalIgnoreCase )
                && vv[ vv.Length - 1 ] == '$' ) {
                vv = vv.Substring( RevTag.Length, vv.Length - RevTag.Length - 1 ).Trim();
                int revision;
                if ( !int.TryParse( vv, NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out revision ) ) {
                    return -1;
                }
                return revision;
            }
            return 0;
        }
        internal static string GetValue( this LdmlAttributeValue[] attributes, LdmlAttribute name ) {
            if ( attributes != null ) {
                for ( int i = 0; i < attributes.Length; i++ ) {
                    if ( attributes[ i ].Name == name )
                        return attributes[ i ].Value;
                }
            }
            return null;
        }
        internal static bool EqualAttributes( LdmlAttributeValue[] a, LdmlAttributeValue[] b ) {
            int length;
            if ( a == null || ( length = a.Length ) == 0 ) {
                return b == null || b.Length == 0;
            }
            if ( b == null || b.Length != length ) {
                return false;
            }
            for ( int i = 0; i < a.Length; i++ ) {
                string val = b.GetValue( a[ i ].Name );
                if ( val != a[ i ].Value ) {
                    return false;
                }
            }
            return true;
        }
        internal static LdmlAttribute GetAttribute( string name ) {
            LdmlAttribute attrName;
            if ( !Enum.TryParse( name, true, out attrName ) ) {
                throw new ArgumentException( "Unknown attribute " + name, nameof(name) );
            }
            return attrName;
        }
        internal static void SkipElement( this XmlReader reader ) {
            if( !reader.IsEmptyElement ) {
                int level = 0;
                while( reader.Read() ) {
                    switch( reader.NodeType ) {
                    case XmlNodeType.Element:
                        if( !reader.IsEmptyElement ) {
                            level++;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        level--;
                        if( level < 0 ) {
                            return;
                        }
                        break;
                    }
                }
            }
        }

        internal static void LoadNode<T>( this XmlReader reader, T elm ) where T : SimpleNode {
            if ( reader.MoveToFirstAttribute() ) {
                while ( true ) {
                    elm.LoadAttribute( reader.Name, reader.Value );
                    if ( !reader.MoveToNextAttribute() ) {
                        break;
                    }
                }
                reader.MoveToElement();
            }
        }
        internal static List<T> LoadList<T>( this XmlReader reader ) where T : SimpleNode, new() {
            var list = new List<T>();
            while ( reader.Read() ) {
                switch ( reader.NodeType ) {
                case XmlNodeType.Element:
                    T elm = new T();
                    LoadNode( reader, elm );
                    list.Add( elm );
                    SkipElement( reader );
                    break;
                case XmlNodeType.EndElement:
                    return list;
                }
            }
            return null;
        }

        internal static void LoadElements( this XmlReader reader, Func<XmlReader,bool> elmReader ) {
            while( reader.Read() ) {
                var type = reader.NodeType;
                if( type == XmlNodeType.Element ) {
                    if ( !elmReader( reader ) ) {
                        reader.SkipElement();
                    }
                } else if( type == XmlNodeType.EndElement ) {
                    break;
                }
            }
        }

        internal static void LoadElements<T>( this XmlReader reader, T owner, Func<XmlReader,T, bool> elmReader ) {
            while( reader.Read() ) {
                var type = reader.NodeType;
                if( type == XmlNodeType.Element ) {
                    if( !elmReader( reader, owner ) ) {
                        reader.SkipElement();
                    }
                } else if( type == XmlNodeType.EndElement ) {
                    break;
                }
            }
        }
        internal static bool AddAttributes( this XmlReader reader, List<AttributeValue> list ) {
            if( reader.MoveToFirstAttribute() ) {
                while( true ) {
                    list.Add( new AttributeValue( reader.Name, reader.Value ) );
                    if( !reader.MoveToNextAttribute() ) {
                        break;
                    }
                }
                reader.MoveToElement();
                return true;
            }
            return false;
        }

        internal static void LoadNodes<T>( this XmlReader reader, T obj, 
            Action<T,string, List<AttributeValue>> nodeAction, string nodeName = null ) {
            var list = new List<AttributeValue>();
            while( reader.Read() ) {
                switch( reader.NodeType ) {
                case XmlNodeType.Element:
                    string elmName = reader.Name;
                    if ( nodeName == null || nodeName == elmName ) {
                        list.Clear();
                        reader.AddAttributes( list );
                        nodeAction( obj, elmName, list );
                    }
                    SkipElement( reader );
                    break;
                case XmlNodeType.EndElement:
                    return;
                }
            }
        }

        internal static void LoadNodes( this XmlReader reader, Action<string, List<AttributeValue>> nodeAction ) {
            var list = new List<AttributeValue>();
            while ( reader.Read() ) {
                switch ( reader.NodeType ) {
                case XmlNodeType.Element:
                    string elmName = reader.Name;
                    list.Clear();
                    reader.AddAttributes( list );
                    nodeAction( elmName, list );
                    SkipElement( reader );
                    break;
                case XmlNodeType.EndElement:
                    return ;
                }
            }
        }
    }
}
