namespace ecl.Unicode.Ucd {
    /// <summary>
    /// Word_Break Property Values
    /// </summary>
    /// <remarks>
    /// http://unicode.org/reports/tr29/#Table_Word_Break_Property_Values
    /// </remarks>
    public enum WordBreak : byte {
        Other,
        DoubleQuote,
        SingleQuote,
        HebrewLetter,
        Cr,
        Lf,
        Newline,
        Extend,
        RegionalIndicator,
        Format,
        Katakana,
        ALetter,
        MidLetter,
        MidNum,
        MidNumLet,
        Numeric,
        ExtendNumLet,
        /// <summary>
        /// 0x200D
        /// </summary>
        ZeroWidthJoiner,
        WSegSpace,

    }
}
