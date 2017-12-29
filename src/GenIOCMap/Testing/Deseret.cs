using System;
namespace GenIOCMap.Testing {
    /// <summary>
    /// is a Unicode block containing characters in the Deseret alphabet, 
    /// which were invented by the The Church of Jesus Christ of Latter-day Saints (LDS Church) 
    /// to write English
    /// </summary>
    /// <remarks>
    /// It appears that OrdinalIgnoreCase GetHashCode algorithm converts Deseret's lower case letters
    /// to upper, while Equals and Compare are not
    /// </remarks>
    class Deseret {

        public void Show() {
            const int FirstUpper = 0x10400;
            const int Length = 0x28;
            const int FirstLower = FirstUpper+Length;

            for ( int i = 0; i < Length; i++ ) {
                string upper = char.ConvertFromUtf32( FirstUpper + i );
                string lower = char.ConvertFromUtf32( FirstLower + i );

                bool eq = string.Equals( upper, lower, StringComparison.OrdinalIgnoreCase );
                int cmp = string.Compare( upper, lower, StringComparison.OrdinalIgnoreCase );
                int hcUpper = StringComparer.OrdinalIgnoreCase.GetHashCode( upper );
                int hcLower = StringComparer.OrdinalIgnoreCase.GetHashCode( lower );

                if ( !eq && cmp != 0 && hcUpper == hcLower ) {
                    Console.WriteLine($"U+{FirstUpper + i:X6}, U+{FirstLower + i:X6}' HashCode = {hcUpper}");
                } else {
                    throw new NotImplementedException("Unreached");
                }
            }
        }
    }
}
