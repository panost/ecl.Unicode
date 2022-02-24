using System;

namespace ecl.Unicode.Ucd {
    public class UcdBlock : UcdRange, IComparable<UcdBlock>{
        public readonly string Name;

        internal UcdBlock( int begin, int end, string name )
            : base( begin, end ) {
            Name = name;
        }

        internal UcdBlock( string name ) {
            Name = name;
        }

        public Block Block => (Block)Begin;

        //public static readonly UcdBlock None = new UcdBlock( "None" );

        //public IEnumerable<UnicodeEntry> GetCodePoints() {
        //    return _context.GetCodePoints( Begin, End );
        //}
        int IComparable<UcdBlock>.CompareTo( UcdBlock other ) {
            return base.CompareTo( other );
        }

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
