using System;

namespace ecl.Unicode.Cldr {
    struct EnumKeyEntry : IEquatable<EnumKeyEntry>, IComparable<EnumKeyEntry> {
        public readonly int Type;
        public readonly string Text;
        public EnumKeyEntry( int type, string text ) {
            Type = type;
            Text = text;
        }

        public bool Equals( EnumKeyEntry other ) {
            return other.Type == Type
                   && Text.SameName( other.Text );
        }

        public int CompareTo( EnumKeyEntry other ) {
            int c = Type - other.Type;
            if ( c != 0 ) {
                return c;
            }
            return StringComparer.OrdinalIgnoreCase.Compare( Text, other.Text );
        }

        public override int GetHashCode() {
            int hc = Type;
            if ( Text.HasValue() ) {
                hc += hc << 3 + StringComparer.OrdinalIgnoreCase.GetHashCode( Text );
            }
            return hc;
        }
    }
}
