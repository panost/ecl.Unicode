using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ecl.Unicode;

namespace eclUnicode.Ucd {
    partial class UcdLoader {
        protected bool TryParseCodeRange( string start, out int begin, out int end ) {
            string stop = null;
            int index = start.IndexOf( "..", StringComparison.Ordinal );
            if( index > 0 ) {
                stop = start.Substring( index + 2 );
                start = start.Substring( 0, index );
            }
            if( !TryParseHex( start, out begin ) ) {
                end = 0;
                return false;
            }

            if( stop.HasValue() ) {
                if( !TryParseHex( stop, out end ) ) {
                    return false;
                }

            } else {
                end = begin;
            }
            return true;
        }
        private IEnumerable<NamedRange> GetNamedRanges( LineReader reader ) {
                List<string> segs = new List<string>();
            foreach( var count in reader.GetLines( segs, 2 ) ) {
                int begin;
                int end;
                if( TryParseCodeRange( segs[ 0 ], out begin, out end ) ) {
                    yield return new NamedRange( begin, end, segs[ 1 ] );
                }
            }
        }

        abstract class ScriptLoaderBase {
            private readonly Dictionary<string, WritingScript> _scripts;
            List<int> extScripts = new List<int>();

            protected readonly UcdLoader _owner;

            protected ScriptLoaderBase( UcdLoader owner ) {
                _owner = owner;
                _scripts = XUtil.GetScriptMap();
            }

            protected abstract void SetScriptsToCode( int code, int last, WritingScript script, bool standard );
            protected abstract void SetScriptRangeToCode( int code, int last, int index, byte count, bool standard );

            public void LoadScripts( LineReader reader, bool standard ) {
                string line;
                List<string> segs = new List<string>();
                List<int> commons = new List<int>();

                foreach ( var count in reader.GetLines( segs, 2 ) ) {
                    line = segs[ 1 ];
                    WritingScript script;
                    commons.Clear();
                    if ( !_scripts.TryGetValue( line, out script ) ) {
                        if ( standard || line.IndexOf( ' ' ) < 0 ) {
                            _owner.Error( $"Unable to find script '{line}'" );
                        } else {
                            foreach ( var s in line.SplitAtSpaces() ) {
                                if ( !_scripts.TryGetValue( s, out script ) ) {
                                    _owner.Error( $"Unable to find script '{s}'" );
                                } else {
                                    commons.Add( (int)script );
                                }
                            }
                            //Debug.WriteLine( line );
                        }
                    } else if (!standard) {
                        commons.Add( (int)script );
                    }
                    int cIndex = extScripts.IndexOf( commons );
                    if ( cIndex < 0 ) {
                        cIndex = extScripts.Count;
                        extScripts.AddRange( commons );
                    }
                    line = segs[ 0 ];
                    int from;
                    int to;
                    int idx = line.IndexOf( "..", StringComparison.Ordinal );
                    if( idx > 0 ) {
                        string text = line.Substring( idx + 2 );
                        if ( !TryParseHex( text, out to ) ) {
                            _owner.Error( "Invalid code value '{0}'", text );
                        }
                        text = line.Substring( 0, idx ).Trim();
                        if ( !TryParseHex( text, out from ) ) {
                            _owner.Error( "Invalid code value '{0}'", text );
                        }
                        if ( from > _owner._maxCodePoint ) {
                            continue;
                        }
                        
                    } else {
                        if ( !TryParseHex( line, out from ) ) {
                            _owner.Error( "Invalid code value '{0}'", line );
                        } else {
                            if ( from > _owner._maxCodePoint ) {
                                continue;
                            }
                        }
                        to = from;
                    }
                    if( standard ) {
                        SetScriptsToCode( from, to, script, standard );
                    } else {
                        SetScriptRangeToCode( from, to, cIndex, ( byte)commons.Count, standard );

                    }
                }
            }

        }

        class ScriptCodeLoaded : ScriptLoaderBase {
            public ScriptCodeLoaded( UcdLoader owner )
                : base( owner ) {
            }

            protected override void SetScriptsToCode( int code, int last, WritingScript script, bool standard ) {
                for ( ; code <= last; code++ ) {
                    var idx = _owner.IndexOf( code );
                    if ( idx >= 0 ) {
                        if ( standard ) {
                            _owner._entries[ idx ].Script = script;
                        }
                    }
                }
            }

            protected override void SetScriptRangeToCode( int code, int last, int index, byte count, bool standard ) {
                for( ; code <= last; code++ ) {
                    var idx = _owner.IndexOf( code );
                    if( idx >= 0 ) {
                        if( standard ) {
                            _owner._entries[ idx ].ScriptIndex = index;
                            _owner._entries[ idx ].ScriptCount = count;
                        }
                    }
                }
            }
        }

        class ScriptLoader : ScriptLoaderBase {
            private readonly Dictionary<int, WritingScript> _scriptMap = new Dictionary<int, WritingScript>();
            private HashSet<int> _inherited = new HashSet<int>();
            public ScriptLoader( UcdLoader owner )
                : base( owner ) {
            }

            protected override void SetScriptsToCode( int code, int last, WritingScript script, bool standard ) {
                for ( ; code <= last; code++ ) {
                    WritingScript otherScript;
                    if( !_scriptMap.TryGetValue( code, out otherScript ) ) {
                        _scriptMap.Add( code, script );
                    } else if( standard ) {
                        _owner.Error( "Duplicate code value 0x{0:X} with script {1}", code, script.ToString() );
                    } else {
                        if( otherScript == WritingScript.Inherited ) {
                            _scriptMap[ code ] = script;
                            _inherited.Add( code );
                        }
                    }
                }
            }

            protected override void SetScriptRangeToCode( int code, int last, int index, byte count, bool standard ) {
                throw new NotImplementedException();
            }
        }
        public void EnsureScriptsLoaded() {
            LoadScripts();
        }
        

        public void LoadScripts() {
            ScriptLoader ldr = new ScriptLoader( this );
            using ( LineReader reader = OpenLineReader( "Scripts.txt" ) ) {
                ldr.LoadScripts( reader, true );
            }
            using ( LineReader reader = OpenLineReader( "ScriptExtensions.txt" ) ) {
                ldr.LoadScripts( reader, false );
            }
        }
    }
}
