namespace eclUnicode.Ucd {
    public enum UnicodeCharacterType : byte {
        /// <summary>
        /// 'Lu'
        /// </summary>
        LetterUppercase,

        /// <summary>
        /// 'Ll'
        /// </summary>
        LetterLowercase,

        /// <summary>
        /// 'Lt'
        /// </summary>
        LetterTitlecase,

        /// <summary>
        /// 'Lm'
        /// </summary>
        LetterModifier,

        /// <summary>
        /// 'Lo'
        /// </summary>
        LetterOther,

        /// <summary>
        /// 'Mn'
        /// </summary>
        MarkNonSpacing,

        /// <summary>
        /// 'Mc'
        /// </summary>
        MarkSpacingCombining,

        /// <summary>
        /// 'Me'
        /// </summary>
        MarkEnclosing,

        /// <summary>
        /// 'Nd'
        /// </summary>
        NumberDecimalDigit,

        /// <summary>
        /// 'Nl'
        /// </summary>
        NumberLetter,

        /// <summary>
        /// 'No'
        /// </summary>
        NumberOther,

        /// <summary>
        /// 'Zs'
        /// </summary>
        SeparatorSpace,

        /// <summary>
        /// 'Zl'
        /// </summary>
        SeparatorLine,

        /// <summary>
        /// 'Zp'
        /// </summary>
        SeparatorParagraph,

        /// <summary>
        /// 'Cc'
        /// </summary>
        OtherControl,

        /// <summary>
        /// 'Cf'
        /// </summary>
        OtherFormat,

        /// <summary>
        /// 'Cs'
        /// </summary>
        OtherSurrogate,

        /// <summary>
        /// 'Co'
        /// </summary>
        OtherPrivateUse,

        /// <summary>
        /// 'Cn'
        /// </summary>
        /// <remarks>
        /// no characters in the file have this property
        /// </remarks>
        OtherNotAssigned,

        /// <summary>
        /// 'Pc'
        /// </summary>
        PunctuationConnector,

        /// <summary>
        /// 'Pd'
        /// </summary>
        PunctuationDash,

        /// <summary>
        /// 'Ps'
        /// </summary>
        PunctuationOpen,

        /// <summary>
        /// 'Pe'
        /// </summary>
        PunctuationClose,

        /// <summary>
        /// 'Pi'
        /// <para>may behave like Ps or Pe depending on usage</para>
        /// </summary>
        PunctuationInitialQuote,

        /// <summary>
        /// 'Pf'
        /// <para>may behave like Ps or Pe depending on usage</para>
        /// </summary>
        PunctuationFinalQuote,

        /// <summary>
        /// 'Po'
        /// </summary>
        PunctuationOther,

        /// <summary>
        /// 'Sm'
        /// </summary>
        SymbolMath,

        /// <summary>
        /// 'Sc'
        /// </summary>
        SymbolCurrency,

        /// <summary>
        /// 'Sk'
        /// </summary>
        SymbolModifier,

        /// <summary>
        /// 'So'
        /// </summary>
        SymbolOther,

    };
}
