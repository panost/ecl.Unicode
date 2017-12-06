using System;

namespace eclUnicode.Cldr.Doc {
    [Flags]
    public enum CldrLoadOptions {
        Normal,

        /// <summary>
        /// Load Rule-Based Number Formatting file
        /// </summary>
        Rbnf=1,

    }
}
