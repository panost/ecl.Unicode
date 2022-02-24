using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace ecl.Unicode.Cldr.Doc {
    /// <summary>
    /// LOCALE DATA MARKUP LANGUAGE (LDML) document
    /// </summary>
    public abstract class LdmlDocument : IEqualityComparer<LdmlNode> {
        private LdmlDocument _parent;
        /// <summary>
        /// 
        /// </summary>
        public LdmlDocument Parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }
        private readonly Dictionary<string, CldrUnit> _units = new Dictionary<string, CldrUnit>( StringComparer.OrdinalIgnoreCase );

        internal CldrUnit GetOrCreateUnit( string name ) {
            CldrUnit unit;
            if ( !_units.TryGetValue( name, out unit ) ) {
                unit = new CldrUnit() {
                    Name = name
                };
                _units.Add( name, unit );
            }
            return unit;
        }
        private readonly Dictionary<string, LdmlNode> _nodes = new Dictionary<string, LdmlNode>( StringComparer.Ordinal );
        private Dictionary<LdmlNode, List<LdmlNode>> _parencyNodes;

        internal void AddRootNode( LdmlNode node ) {
            _nodes[ node.Name ] = node;
        }
        internal LdmlNode GetRootNode( string name ) {
            LdmlNode node;
            _nodes.TryGetValue( name, out node );
            return node;
        }
        internal LdmlNode ResolveRootNode( string name ) {
            LdmlNode node;
            if ( !_nodes.TryGetValue( name, out node ) && _parent != null ) {
                return _parent.ResolveRootNode( name );
            }
            return node;
        }
        internal LdmlNode SelectNode( params string[] entries ) {
            LdmlNode root = ResolveRootNode( entries[ 0 ] );
            for ( int i = 1; root != null && i < entries.Length; i++ ) {
                root = root.Select( entries[ i ] );
            }
            return root;
        }
        internal LdmlNode SelectRootNode( string name, params NodePathEntry[] entries ) {
            return ResolveRootNode( name ).Select( entries );
        }
        public string SelectNodeText( params string[] entries ) {
            return SelectNode( entries ).GetText();
        }
        public string SelectNodeKeyText( string keyValue, LdmlAttribute attr, params string[] entries ) {
            LdmlNode root = SelectNode( entries );
            if ( root != null ) {
                foreach ( LdmlNode node in root.Children ) {
                    if ( node.GetAttribute( attr ).SameName( keyValue ) ) {
                        return node.Value ?? "";
                    }
                }
            }
            return "";
        }
        internal LdmlNode ResolveNode( params NodePathEntry[] entries ) {
            LdmlNode root = ResolveRootNode( entries[ 0 ].Name );
            for ( int i = 1; root != null && i < entries.Length; i++ ) {
                root = root.ResolveNode( entries[ i ].Name, entries[ i ].Attributes );
            }
            return root;
        }
        internal void SetRootNode( LdmlNode cloned ) {
            _nodes.Add( cloned.Name, cloned );
        }

        public readonly CldrLoader Loader;
        public readonly string Name;

        protected LdmlDocument( CldrLoader loader, LdmlDocument parent, string name ) {
            _nodes[ "" ] = _nodes[ "/" ] = null;
            _parent = parent;
            Loader = loader;
            Name = name;
            _parencyNodes = new Dictionary<LdmlNode, List<LdmlNode>>( this );
        }
        protected void AppendPath( StringBuilder b ) {
            if ( Parent != null ) {
                Parent.AppendPath( b );
                b.Append( '/' );
                b.Append( Name );
            } else {
                b.Append( '~' );
            }
        }

        protected virtual void AppendString( StringBuilder b ) {
            if ( _parent == null ) {
                b.Append( Name );
                return;
            }
            b.Append( " path:" );
            AppendPath( b );
        }
        public override string ToString() {
            StringBuilder b = new StringBuilder();
            AppendString( b );
            return b.ToString();
        }


        



        internal LdmlNode SelectNode( string path ) {
            LdmlNode node;
            if ( !_nodes.TryGetValue( path, out node ) ) {
                NodePath p = new NodePath( path );
                return p.SelectNode( this );

            }
            return node;
        }

        internal LdmlNode[] SelectNodes( string path ) {
            var node = SelectNode( path );
            if ( node != null ) {
                return node.Children;
            }
            return null;
        }
        internal void SetParent( LdmlNode child, LdmlNode parent ) {
            _parencyNodes.AddGroup( parent, child );
        }

        
        protected void Load( XmlReader reader ) {
            reader.MoveToContent();
            if ( reader.IsEmptyElement ) {
                return;
            }
            while ( reader.Read() ) {
                var type = reader.NodeType;
                if ( type == XmlNodeType.Element ) {
                    LdmlNode node = ReadRootElement( reader );
                    if ( node != null ) {
                        AddRootNode( node );
                    }
                } else if ( type == XmlNodeType.EndElement ) {
                    break;
                }
            }
            
        }

        internal virtual void Loaded() {
            foreach ( KeyValuePair<LdmlNode, List<LdmlNode>> pair in _parencyNodes ) {
                pair.Key._nodes = pair.Value.ToArray();
            }
            if ( _pendingAlias != null ) {
                ResolveAlias();
            }
            if ( _parent != null ) {
                ComputeAliveAlias();
            }
            foreach ( KeyValuePair<LdmlNode, List<LdmlNode>> pair in _parencyNodes ) {
                if ( pair.Key._nodes.Length != pair.Value.Count ) {
                    pair.Key._nodes = pair.Value.ToArray();
                }
            }
            _parencyNodes = null;
        }

        

        private void ComputeAliveAlias() {
            List<LdmlNode> all = new List<LdmlNode>();
            for ( var doc = this; doc != null; doc = doc._parent ) {
                if ( doc._localAlias != null ) {
                    all.AddRange( doc._localAlias );
                }
            }
            //List<LdmlNode> alive = new List<LdmlNode>();
            foreach ( LdmlNode node in all ) {
                var target = node.AliasTarget;
                if ( target != null ) {
                    var myTarget = target.FindCousin( this );
                    if ( myTarget != null ) {
                        // everything that's alive and not exists here should be cloned
                        LdmlNode myNode = node.FindCousin( this );
                        if ( myNode == null ) {
                            myNode = node.Clone( this );
                            //alive.Add( node );
                        }
                        myNode.AliasTarget = myTarget;
                    }
                }
            }
        }

        internal abstract LdmlNode ReadRootElement( XmlReader reader );

        internal LdmlNode ReadNode( XmlReader reader, LdmlNode parent ) {
            string name = Loader.GetCachedName( reader.Name );

            LdmlNode node = parent.CreateChildNode( name );
            node.Load( name, this, reader, parent );
            SetParent( node, parent );
            return node;
        }

        internal virtual LdmlNode CreateNode( string cname, LdmlNode parent ) {
            return new LdmlAnyNode();
        }

        private Dictionary<LdmlNode, NodePath> _pendingAlias;
        private List<LdmlNode> _localAlias;

        private void ResolveAlias() {
            _localAlias = new List<LdmlNode>();
            foreach ( var pair in _pendingAlias ) {
                LdmlNode targetNode = pair.Value.ResolveNode( pair.Key );
                if ( targetNode == null ) {
                    Debug.WriteLine( pair.Value.Path );
                } else {
                    pair.Key.AliasTarget = targetNode;
                    _localAlias.Add( pair.Key );
                }
            }
            _pendingAlias = null;
        }


        internal void ReadAlias( XmlReader reader, LdmlNode node ) {
            string source = null;
            string path = null;
            bool alt = false;
            if ( reader.MoveToFirstAttribute() ) {

                while ( true ) {
                    switch ( reader.Name ) {
                    case "source":
                        source = reader.Value;
                        break;
                    case "path":
                        path = reader.Value;
                        break;
                    default:
                        alt = true;
                        break;
                    }
                    if ( !reader.MoveToNextAttribute() ) {
                        break;
                    }
                }
                reader.MoveToElement();
                NodePath alias = new NodePath( path );
                reader.SkipElement();
                if ( _pendingAlias == null ) {
                    _pendingAlias = new Dictionary<LdmlNode, NodePath>( this );
                }
                _pendingAlias.Add( node, alias );
            }
            reader.SkipElement();
        }

        bool IEqualityComparer<LdmlNode>.Equals( LdmlNode x, LdmlNode y ) {
            return ReferenceEquals( x, y );
        }

        int IEqualityComparer<LdmlNode>.GetHashCode( LdmlNode obj ) {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode( obj );
        }

        
    }
}
