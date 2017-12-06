namespace eclUnicode.Ucd {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// http://unicode.org/reports/tr9/#Bidirectional_Character_Types
    /// </remarks>
    public enum BidirectionalCategory : byte {
        /// <summary>
        /// 'L'
        /// </summary>
        LeftToRight,

        /// <summary>
        /// 'R'
        /// </summary>
        RightToLeft,

        /// <summary>
        /// 'AL'
        /// </summary>
        ArabicLetter,

        /// <summary>
        /// 'EN'
        /// </summary>
        EuropeanNumber,

        /// <summary>
        /// 'ES'
        /// EuropeanNumberSeparator
        /// </summary>
        EuropeanSeparator,

        /// <summary>
        /// 'ET'
        /// EuropeanNumberTerminator
        /// </summary>
        EuropeanTerminator,

        /// <summary>
        /// 'AN'
        /// </summary>
        ArabicNumber,

        /// <summary>
        /// 'CS'
        /// CommonNumberSeparator
        /// </summary>
        CommonSeparator,

        /// <summary>
        /// 'NSM'
        /// </summary>
        NonSpacingMark,

        /// <summary>
        /// 'BN'
        /// </summary>
        BoundaryNeutral,

        /* Neutral */

        /// <summary>
        /// 'B'
        /// </summary>
        ParagraphSeparator,

        /// <summary>
        /// 'S'
        /// </summary>
        SegmentSeparator,

        /// <summary>
        /// 'WS'
        /// </summary>
        WhiteSpace,

        /// <summary>
        /// 'ON'
        /// </summary>
        OtherNeutral,

        /* Explicit Formatting */
        /// <summary>
        /// 'LRE'
        /// </summary>
        LeftToRightEmbedding,

        /// <summary>
        /// 'LRO'
        /// </summary>
        LeftToRightOverride,



        /// <summary>
        /// 'RLE'
        /// </summary>
        RightToLeftEmbedding,

        /// <summary>
        /// 'RLO'
        /// </summary>
        RightToLeftOverride,

        /// <summary>
        /// 'PDF'
        /// </summary>
        PopDirectionalFormat,

        //---------
        /// <summary>
        /// 'LRI'
        /// </summary>
        LeftToRightIsolate,



        /// <summary>
        /// 'RLI'
        /// </summary>
        RightToLeftIsolate,

        /// <summary>
        /// 'FSI'
        /// </summary>
        FirstStrongIsolate,

        /// <summary>
        /// 'PDI'
        /// </summary>
        PopDirectionalIsolate,


    }
}
