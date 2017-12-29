using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ecl.Unicode.Cldr.Doc {
    public abstract class LdmlNode : NamedObject, IEquatable<LdmlNode> {
        //internal int _nodeId;
        public static readonly LdmlNode[] EmptyArray = new LdmlNode[ 0 ];

        private string _value;
        /// <summary>
        /// 
        /// </summary>
        public string Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

        private LdmlDocument _document;
        /// <summary>
        /// 
        /// </summary>
        public LdmlDocument Document {
            get {
                return _document;
            }
            set {
                _document = value;
            }
        }

        private DraftStatus _draftStatus;
        /// <summary>
        /// 
        /// </summary>
        public DraftStatus DraftStatus {
            get { return _draftStatus; }
        }

        //public readonly string Type;
        //public readonly string Alt;
        private LdmlAttributeValue[] _attributes;
        /// <summary>
        /// 
        /// </summary>
        public LdmlAttributeValue[] Attributes {
            get {
                return _attributes;
            }
            protected set {
                _attributes = value;
            }
        }

        private LdmlNode _parent;
        /// <summary>
        /// 
        /// </summary>
        internal LdmlNode Parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }

        //private NodePath _alias;
        ///// <summary>
        ///// 
        ///// </summary>
        //internal NodePath Alias {
        //    get {
        //        return _alias;
        //    }
        //    set {
        //        _alias = value;
        //    }
        //}

        internal LdmlNode[] _nodes = EmptyArray;

        public IEnumerable<T> ChildrenOf<T>() where T: LdmlNode {
            var len = Children.Length;
            for ( int i = 0; i < len; i++ ) {
                T node = _children[ i ] as T;
                if ( node != null ) {
                    yield return node;
                }
            }
        }
        private LdmlNode[] _children;
        /// <summary>
        /// 
        /// </summary>
        public LdmlNode[] Children {
            get {
                if ( _children == null ) {
                    _children = ComputeChildren();
                }
                return _children;
            }
        }
        private LdmlNode[] ComputeChildren() {
            LdmlNode[][] stack = new LdmlNode[3][];
            int length = 0;
            if ( _nodes.Length > 0 ) {
                stack[ length++ ] = _nodes;
            }
            if ( AliasTarget != null && AliasTarget.Children.Length > 0 ) {
                stack[ length++ ] = AliasTarget.Children;
            }
            for ( var doc = Document.Parent; doc != null; doc = doc.Parent ) {
                var me = FindCousin( doc );
                if ( me != null ) {
                    if ( me.Children.Length > 0 ) {
                        stack[ length++ ] = me.Children;
                    }
                    break;
                }
            }
            if ( length == 0 ) {
                return EmptyArray;
            }
            if ( length == 1 ) {
                return stack[ 0 ];
            }
            var list = new List<LdmlNode>( stack[ 0 ] );
            for ( int i = 1; i < length; i++ ) {
                foreach ( LdmlNode node in stack[i] ) {
                    LdmlNode found = null;
                    foreach ( LdmlNode scanNode in list ) {
                        if ( scanNode.SameNode( node ) ) {
                            found = scanNode;
                            break;
                        }
                    }
                    if ( found == null ) {
                        list.Add( node );
                    }
                }
            }
            if ( list.Count == stack[ 0 ].Length ) {
                return stack[ 0 ];
            }
            //list.Sort( CompareNodes );
            return list.ToArray();
        }

        //private int CompareNodes( LdmlNode a, LdmlNode b ) {
        //    int diff = string.Compare( a.Name, b.Name, StringComparison.Ordinal );
        //    if ( diff == 0 ) {
        //    }
        //    return diff;
        //}

        protected virtual void AppendAttributes( StringBuilder b ) {
            if ( Attributes != null ) {
                b.Append( ' ' );
                foreach ( LdmlAttributeValue attribute in Attributes ) {
                    attribute.AppendString( b );
                }
            }
        }

        protected void AppendString( StringBuilder b ) {
            if ( Name.HasValue() ) {
                b.Append( Name );
            }
            AppendAttributes( b );

        }
        public override string ToString() {
            StringBuilder b = new StringBuilder();
            AppendString( b );
            return b.ToString();
        }

        public string FullName {
            get {
                StringBuilder b = new StringBuilder();
                if ( Parent != null ) {
                    b.Append( Parent.FullName ).Append( '/' );
                }
                AppendString( b );
                return b.ToString();
            }
        }
        public void Load( string nodeName, LdmlDocument document, XmlReader reader, LdmlNode parent ) {
            _document = document;
            _parent = parent;
            _code = nodeName;

            if ( reader.MoveToFirstAttribute() ) {
                List<LdmlAttributeValue> list = null;
                while ( true ) {
                    string name = reader.Name;
                    string value = reader.Value;
                    LdmlAttribute attr = LdmlUtil.GetAttribute( name );
                    if ( attr == LdmlAttribute.Draft ) {
                        _draftStatus = CldrUtil.GetDraftStatus( value );
                    } else if ( !HandleAttribute( attr, value ) ) {
                        if ( list == null ) {
                            list = new List<LdmlAttributeValue>();
                        }
                        list.Add( new LdmlAttributeValue( attr, value ) );
                    }
                    


                    if ( !reader.MoveToNextAttribute() ) {
                        break;
                    }
                }
                reader.MoveToElement();
                if ( list != null ) {
                    Attributes = list.ToArray();
                }

            }
            //ReadChildren( reader );

            if ( reader.IsEmptyElement ) {
                return;
            }
            while ( reader.Read() ) {
                var type = reader.NodeType;
                if ( type == XmlNodeType.Element ) {
                    if ( reader.Name == "alias" ) {
                        _document.ReadAlias( reader, this );
                    } else {
                        _document.ReadNode( reader, this );
                        //_nodes.Add( node );
                    }

                } else if ( type == XmlNodeType.Text ) {
                    _value = reader.Value;
                } else if ( type == XmlNodeType.EndElement ) {
                    break;
                }
            }
        }

        internal virtual LdmlNode CreateChildNode( string name ) {
            return _document.CreateNode( name, this );
        }
        protected virtual bool HandleAttribute( LdmlAttribute attr, string value ) {
            return false;
        }
        

        internal LdmlNode FindCousin( LdmlDocument doc ) {
            if ( Parent == null ) {
                return doc.GetRootNode( Name );
            }
            var parent = Parent.FindCousin( doc );
            if ( parent != null ) {
                foreach ( var node in parent._nodes ) {
                    if ( SameNode( node ) ) {
                        return node;
                    }
                }
            }
            return null;
        }

        public override int GetHashCode() {
            int hc = Name.GetHashCode();
            
            if ( Attributes != null ) {
                foreach ( var attribute in Attributes ) {
                    hc += hc ^ attribute.Name.GetHashCode();
                    hc += hc ^ attribute.Value.GetHashCode();
                }
            }
            return hc;
        }



        public abstract bool SameNode( LdmlNode other );

        public bool Equals( LdmlNode other ) {
            if ( SameNode( other ) ) {
                if ( Parent == null ) {
                    return other.Parent == null;
                }
                if ( other.Parent != null ) {
                    return Parent.Equals( other.Parent );
                }
            }
            return false;
        }



        public abstract string GetAttribute( LdmlAttribute attr );


        
        


        internal virtual bool HasAttributes( LdmlAttributeValue[] filter ) {
            if ( filter != null ) {
                foreach ( LdmlAttributeValue a in filter ) {
                    if ( GetAttribute( a.Name ) != a.Value ) {
                        return false;
                    }
                }
            }
            return true;
        }

        internal LdmlNode ResolveNode( string name, LdmlAttributeValue[] filter = null ) {
            foreach ( LdmlNode node in _nodes ) {
                if ( node.Name == name && node.HasAttributes( filter ) ) {
                    return node;
                }
            }

            for ( var doc = _document.Parent; doc != null; doc = doc.Parent ) {
                var me = FindCousin( doc );
                if ( me != null ) {
                    return me.ResolveNode( name, filter );
                }
            }
            return null;
        }
        internal IEnumerable<LdmlNode> SelectNodes( string name, LdmlAttributeValue[] filter = null ) {
            foreach ( LdmlNode node in Children ) {
                if ( node.Name == name && node.HasAttributes( filter ) ) {
                    yield return node;
                }
            }
        }
        
        

        

        internal string[] GetList( string name, Func<LdmlNode,int> indexer ) {
            var list = new List<IndexedEntry<string>>();

            foreach ( LdmlNode node in Children ) {
                if ( node.Name == name ) {
                    int idx = indexer( node );
                    if ( idx >= 0 ) {
                        IndexedEntry<string> e = new IndexedEntry<string>( idx, node.Value );
                        list.Add( e );
                    }
                }
            }
            list.SortByIndex();
            return list.GetValues();
        }

        private LdmlNode _aliasTarget;
        /// <summary>
        /// 
        /// </summary>
        internal LdmlNode AliasTarget {
            get {
                return _aliasTarget;
            }
            set {
                _aliasTarget = value;
            }
        }


        //public LdmlNode[] XRef {
        //    get;
        //    set;
        //}


        internal LdmlNode Clone( LdmlDocument doc ) {
            LdmlNode cloned = (LdmlNode)MemberwiseClone();
            cloned._document = doc;
            cloned._children = null;
            cloned._nodes = EmptyArray;
            //if ( Name == "dayPeriodWidth" ) {
            //    Debug.WriteLine( Value );
            //}
            if ( _parent != null ) {
                LdmlNode myNode = _parent.FindCousin( doc );
                if ( myNode == null ) {
                    myNode = _parent.Clone( doc );
                }
                cloned._parent = myNode;
                doc.SetParent( cloned, myNode );
                //myNode.Nodes.Add( cloned );
            } else {
                doc.SetRootNode( cloned );
            }
            return cloned;
        }
    }
}
