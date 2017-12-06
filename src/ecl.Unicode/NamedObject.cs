using System.Diagnostics;

namespace ecl.Unicode {
    [DebuggerDisplay( "{GetType().Name} Name={_code}" )]
    public class CodeObjectBase {
        internal string _code;

        /// <summary>
        /// Retrieves the names of a NamedObject array
        /// </summary>
        /// <param name="obj">the array, if null, null is returned</param>
        /// <returns>a string array of the object's names</returns>
        public static string[] GetNames( CodeObjectBase[] obj ) {
            if( obj == null )
                return null;
            string[] names = new string[ obj.Length ];
            for( int i = 0; i < obj.Length; i++ ) {
                names[ i ] = obj[ i ]._code;
            }
            return names;
        }
    }

    public class NamedObject: CodeObjectBase {
        /// <summary>
        /// 
        /// </summary>
        public string Name {
            get {
                return _code;
            }
            set {
                _code = value;
            }
        }
    }
}
