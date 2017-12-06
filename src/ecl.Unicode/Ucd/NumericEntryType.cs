using System.Runtime.InteropServices;

namespace eclUnicode.Ucd {
    enum NumericEntryType : byte {
        None,
        Decimal,
        Digit,
        Value,
        Fraction,
        Billions
    }

    [StructLayout(LayoutKind.Explicit)]
    struct NumericValue {
        [FieldOffset( 0 )]
        public int Value;
        [FieldOffset( 0 )]
        public short Numerator;
        [FieldOffset( 2 )]
        public short Denominator;
    }
}
