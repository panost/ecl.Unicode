namespace ecl.Unicode.Ucd {
    /// <summary>
    /// 
    /// </summary>
    /// <cite>https://unicode.org/reports/tr29/</cite>
    public enum GraphemeClusterBreak : byte {
        Other,
        Prepend,
        CarriageReturn,
        LineFeed,
        Control,
        Extend,
        RegionalIndicator,
        SpacingMark,

        /// <summary>
        /// Hangul_Syllable_Type=L
        /// </summary>
        L,
        V,
        T,
        LV,
        LVT,
        /// <summary>
        /// 'ZWJ'
        /// </summary>
        ZeroWidthJoiner
    }
}
