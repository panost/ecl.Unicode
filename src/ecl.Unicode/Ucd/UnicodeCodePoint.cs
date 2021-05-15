using System;
using System.Collections.Generic;
using System.Text;

namespace ecl.Unicode.Ucd {
    /// <summary>
    /// Some well known unicode points
    /// </summary>
    public enum UnicodeCodePoint {
        Null,
        Space = 32,

        /// <summary>
        /// Used to replace an incoming character whose value is unknown or unrepresentable in Unicode
        /// </summary>
        ReplacementCharacter = 0xFFFD,

        /// <summary>
        /// The last code point defined by the Unicode specification.
        /// </summary>
        Last = 0x10FFFF
    }
}
