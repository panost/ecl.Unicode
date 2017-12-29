using System;

namespace ecl.Unicode.Cldr.Doc {
    [Flags]
    public enum CldrLoadOptions {
        Normal,

        /// <summary>
        /// Load Rule-Based Number Formatting file
        /// </summary>
        Rbnf=1,

    }
}
