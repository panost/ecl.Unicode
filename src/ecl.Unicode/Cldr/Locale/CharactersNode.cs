using System;
using System.Collections.Generic;
using ecl.Unicode;
using eclUnicode.Cldr.Doc;

namespace eclUnicode.Cldr.Locale {
    public class CharactersNode: LdmlNoKeyNode {
        internal override LdmlNode CreateChildNode( string name ) {
            //switch( name ) {
            //case "territories":
            //    return new TerritoriesNode();
            //case "languages":
            //    return new DictionayOwnerNode( "language" );
            //}
            return base.CreateChildNode( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        /// <remarks>
        /// http://unicode.org/reports/tr35/tr35-6.html#Character_Elements
        /// </remarks>
        public string[] GetExemplar( ExemplarType tp = ExemplarType.Standard ) {
            string code;
            switch ( tp ) {
            case ExemplarType.Standard:
                code = null;
                break;
            case ExemplarType.Auxiliary:
                code = "auxiliary";
                break;
            case ExemplarType.Index:
                code = "index";
                break;
            case ExemplarType.Punctuation:
                code = "punctuation";
                break;
            default:
                return null;
            }
            int len;
            var str = this.Select( "exemplarCharacters", code, LdmlAttribute.Type )?.GetText();
            if ( str != null && (len = str.Length) > 2 && str[ 0 ] == '[' && str[ len - 1 ] == ']' ) {
                str = str.Substring( 1, len - 2 );
                var ss = str.SplitAtSpaces();
                List<string> list = new List<string>( ss.Length );
                for ( int i = 0; i < ss.Length; i++ ) {
                    str = ss[ i ];
                    len = str.Length;
                    if ( len == 1 ) {
                        list.Add( str );
                        continue;
                    }
                    if ( len > 2 ) {
                        if ( str[ 0 ] == '{' && str[ len - 1 ] == '}' ) {
                            list.Add( str.Substring( 1, len - 2 ) );
                            continue;
                        }
                        if ( len == 3 && str[ 1 ] == '-' ) {
                            for ( char j = str[ 0 ]; j < str[ 2 ]; j++ ) {
                                list.Add( j.ToString() );
                            }
                            continue;
                        }
                    }
                    throw new NotImplementedException();
                }
                return list.ToArray();
            }

            return null;
        }

        public string GetEllipsis( EllipsisType type ) {
            string code;
            switch( type ) {
            case EllipsisType.final:
                code = "final";
                break;
            case EllipsisType.initial:
                code = "initial";
                break;
            case EllipsisType.medial:
                code = "medial";
                break;
            case EllipsisType.wordFinal:
                code = "word-final";
                break;
            case EllipsisType.wordinitial:
                code = "word-initial";
                break;
            case EllipsisType.wordMedial:
                code = "word-medial";
                break;
            default:
                return null;
            }
            
            foreach( LdmlNode child in Children ) {
                if( child.Name == "exemplarCharacters" ) {
                    if( child.GetAttribute( LdmlAttribute.Type ) == code ) {
                        return child.GetText();
                    }
                }
            }
            return null;
        }
    }
}
