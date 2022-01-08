using System;
using System.Collections.Generic;
using System.Text;
using static ecl.Unicode.Ucd.UcdLoader;
using System.Xml;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.IO;

namespace ecl.Unicode.Cldr.Doc;

partial class CldrLoader {

    ScriptMetadata[] _scriptMetadata;
    public ScriptMetadata[] GetScriptMetadata() {
        return _scriptMetadata ??= LoadScriptMetadata();
    }
    public ScriptMetadata[] LoadScriptMetadata() {
        var list = new List<ScriptMetadata>();
        var scripts = XUtil.GetScriptMap();
        using ( var reader = OpenFile( "properties", "scriptMetadata.txt" ) )
        using ( var txt = new StreamReader( reader ) ) {
            string line;
            while ( ( line = txt.ReadLine() ) != null) { 
                if ( !line.HasValue() || line[ 0 ] == '#' )
                    continue;
                string[] fields = line.Split( ';' );
                string code = fields[ 0 ]?.Trim();
                if ( !code.HasValue() ) {
                    continue;
                }
                if ( !scripts.TryGetValue( code, out WritingScript script ) ) {
                    throw new FormatException( "Id " + code );
                }
                var cur = new ScriptMetadata();
                cur.Script = script;
                list.Add( cur );

                code = fields[ 1 ].Trim();
                if (int.TryParse(code,out int rank)) {
                    cur.Rank = rank;
				}
                code = fields[ 3 ].Trim();
                if ( code.HasValue() ) {
                    cur.Territory = GetTerritory( code );
                }
                code = fields[ 4 ].Trim();
                if ( int.TryParse( code, out rank ) ) {
                    cur.Density = rank;
                }
                
                
            }
        }
        return list.ToArray();
    }
}
