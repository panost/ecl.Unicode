using System;
using ecl.Unicode;

namespace eclUnicode.Ucd {
    public class UcdBlock : UcdRange {
        public readonly string Name;
        internal readonly UcdLoader _context;

        internal UcdBlock( int begin, int end, string name )
            : base( begin, end ) {
            Name = name;
        }

        internal UcdBlock( string name ) {
            Name = name;
        }
        
        //public static readonly UcdBlock None = new UcdBlock( "None" );

        //public IEnumerable<UnicodeEntry> GetCodePoints() {
        //    return _context.GetCodePoints( Begin, End );
        //}
        public override string ToString() {
            if ( Length <= 0 ) {
                return Name ?? "Empty";
            }
            var str = base.ToString();
            if ( Name.HasValue() ) {
                str = Name + ": " + str;
            }
            return str;
        }
    }
}
