using System.Collections.Generic;

namespace ecl.Unicode.Ucd {
    public abstract class UcdContainer {
        public abstract bool Contains( int codePoint );
        //HashSet<int> a;
    }

    public class UcdSet : UcdContainer {
        private readonly List<int> _codePoints = new List<int>();

        public sealed override bool Contains( int codePoint ) {
            return _codePoints.BinarySearch( codePoint ) >= 0;
        }

        public bool Add( int codePoint ) {
            int idx = _codePoints.BinarySearch( codePoint );
            if( idx < 0 ) {
                _codePoints.Insert( ~idx, codePoint );
                return true;
            }
            return false;
        }
        public void Add( int start, int last ) {
            for ( ; start <= last; start++ ) {
                int idx = _codePoints.BinarySearch( start );
                if( idx < 0 ) {
                    idx = ~idx;
                    _codePoints.Insert( ~idx, start );
                }
            }
        }
    }
}
