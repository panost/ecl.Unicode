using System;
using ecl.Unicode.Cldr.Doc;
// ReSharper disable UnusedMember.Global
// ReSharper disable StringLiteralTypo

namespace ecl.Unicode.Ucd {
    /// <summary>
    /// Unicode normal and derived properties
    /// </summary>
    public enum CodePointProperty : byte {
        None,

        /// <summary>
        /// ASCII characters commonly used for the representation of hexadecimal numbers.
        /// </summary>
        [PropCode( "ASCII_Hex_Digit" )]
        AsciiHexDigit,

        /// <summary> Format control characters which have specific functions in the Unicode Bidirectional Algorithm [UAX9].</summary>
        [PropCode( "Bidi_Control" )]
        BidiControl,

        /// <summary> Punctuation characters explicitly called out as dashes in the Unicode Standard, plus their compatibility equivalents. Most of these have the General_Category value Pd, but some have the General_Category value Sm because of their use in mathematics.</summary>
        [PropCode( "Dash" )]
        Dash,

        /// <summary>
        /// For a machine-readable list of deprecated characters.
        /// No characters will ever be removed from the standard,
        /// but the usage of deprecated characters is strongly discouraged.
        /// </summary>
        [PropCode( "Deprecated" )]
        Deprecated,

        /// <summary> Characters that linguistically modify the meaning of another character to which they apply. Some diacritics are not combining characters, and some combining characters are not diacritics.</summary>
        [PropCode( "Diacritic" )]
        Diacritic,

        /// <summary> Characters whose principal function is to extend the value or shape of a preceding alphabetic character. Typical of these are length and iteration marks.</summary>
        [PropCode( "Extender" )]
        Extender,

        /// <summary>
        /// Characters commonly used for the representation of hexadecimal numbers, 
        /// plus their compatibility equivalents.
        /// </summary>
        [PropCode( "Hex_Digit" )]
        HexDigit,

        /// <summary>
        /// of 4.0.0; Deprecated as of 6.0.0
        /// B I Dashes which are used to mark connections between pieces of words,
        /// plus the Katakana middle dot.
        /// The Katakana middle dot functions like a hyphen, but is
        /// shaped like a dot rather than a dash.</summary>
        [PropCode( "Hyphen" )]
        [Obsolete]
        Hyphen,

        /// <summary> Characters considered to be CJKV (Chinese, Japanese, Korean, and Vietnamese) ideographs. This property roughly defines the class of "Chinese characters" and does not include characters of other logographic scripts such as Cuneiform or Egyptian Hieroglyphs.</summary>
        [PropCode( "Ideographic" )]
        Ideographic,

        /// <summary> Used in Ideographic Description Sequences.</summary>
        [PropCode( "IDS_Binary_Operator" )]
        IdsBinaryOperator,

        /// <summary> Used in Ideographic Description Sequences.</summary>
        [PropCode( "IDS_Trinary_Operator" )]
        IdsTrinaryOperator,

        /// <summary> Format control characters which have specific functions for control of cursive joining and ligation.</summary>
        [PropCode( "Join_Control" )]
        JoinControl,

        /// <summary> A small number of spacing vowel letters occurring in certain Southeast Asian scripts such as Thai and Lao, which use a visual order display model. These letters are stored in text ahead of syllable-initial consonants, and require special handling for processes such as searching and sorting.</summary>
        [PropCode( "Logical_Order_Exception" )]
        LogicalOrderException,

        /// <summary> Code points permanently reserved for internal use.</summary>
        [PropCode( "Noncharacter_Code_Point" )]
        NonCharacterCodePoint,

        /// <summary> Used in deriving the Alphabetic property.</summary>
        [PropCode( "Other_Alphabetic" )]
        OtherAlphabetic,

        /// <summary> Used in deriving the Default_Ignorable_Code_Point property.</summary>
        [PropCode( "Other_Default_Ignorable_Code_Point" )]
        OtherDefaultIgnorableCodePoint,

        /// <summary> Used in deriving  the Grapheme_Extend property.</summary>
        [PropCode( "Other_Grapheme_Extend" )]
        OtherGraphemeExtend,

        /// <summary> Used to maintain backward compatibility of ID_Continue.</summary>
        [PropCode( "Other_ID_Continue" )]
        OtherIdContinue,

        /// <summary> Used to maintain backward compatibility of ID_Start.</summary>
        [PropCode( "Other_ID_Start" )]
        OtherIdStart,

        /// <summary> Used in deriving the Lowercase property.</summary>
        [PropCode( "Other_Lowercase" )]
        OtherLowercase,

        /// <summary> Used in deriving the Math property.</summary>
        [PropCode( "Other_Math" )]
        OtherMath,

        /// <summary> Used in deriving the Uppercase property.</summary>
        [PropCode( "Other_Uppercase" )]
        OtherUppercase,

        /// <summary> Used for pattern syntax as described in Unicode Standard Annex #31, "Unicode Identifier and Pattern Syntax" [UAX31].</summary>
        [PropCode( "Pattern_Syntax" )]
        PatternSyntax,

        /// <summary> Punctuation characters that function as quotation marks.</summary>
        [PropCode( "Quotation_Mark" )]
        QuotationMark,

        /// <summary> Used in Ideographic Description Sequences.</summary>
        [PropCode( "Radical" )]
        Radical,

        /// <summary> Characters with a "soft dot", like i or j. An accent placed on these characters causes the dot to disappear. An explicit dot above can be added where required, such as in Lithuanian.</summary>
        [PropCode( "Soft_Dotted" )]
        SoftDotted,

        /// <summary> Sentence Terminal. Used in Unicode Standard Annex #29, "Unicode Text Segmentation" [UAX29].</summary>
        [PropCode( "STerm" )]
        STerm,

        /// <summary> Punctuation characters that generally mark the end of textual units.</summary>
        [PropCode( "Terminal_Punctuation" )]
        TerminalPunctuation,

        /// <summary> A property which specifies the exact set of Unified CJK Ideographs in the standard. This set excludes CJK Compatibility Ideographs (which have canonical decompositions to Unified CJK Ideographs), as well as characters from the CJK Symbols and Punctuation block. The property is used in the definition of Ideographic Description Sequences.</summary>
        [PropCode( "Unified_Ideograph" )]
        UnifiedIdeograph,

        /// <summary> Indicates characters that are Variation Selectors. For details on the behavior of these characters, see StandardizedVariants.html, Section 23.4, Variation Selectors in [Unicode], and Unicode Technical Standard #37, "Unicode Ideographic Variation Database" [UTS37].</summary>
        [PropCode( "Variation_Selector" )]
        VariationSelector,

        /// <summary> Spaces, separator characters and other control characters which should be treated by programming languages as "white space" for the purpose of parsing elements. See also Line_Break, Grapheme_Cluster_Break, Sentence_Break, and Word_Break, which classify space characters and related controls somewhat differently for particular text segmentation contexts.</summary>
        [PropCode( "White_Space" )]
        WhiteSpace,

        [PropCode( "Sentence_Terminal" )]
        SentenceTerminal,

        [PropCode( "Pattern_White_Space" )]
        PatternWhiteSpace,

        [PropCode( "Prepended_Concatenation_Mark" )]
        PrependedConcatenationMark,

        [PropCode( "Regional_Indicator" )]
        RegionalIndicator,

        [PropCode( "Math", true )]
        Math,

        [PropCode( "Alphabetic", true )]
        Alphabetic,

        [PropCode( "Lowercase", true )]
        Lowercase,

        [PropCode( "Uppercase", true )]
        Uppercase,

        [PropCode( "Cased", true )]
        Cased,

        [PropCode( "Case_Ignorable", true )]
        CaseIgnorable,

        [PropCode( "Changes_When_Lowercased", true )]
        ChangesWhenLowercased,

        [PropCode( "Changes_When_Uppercased", true )]
        ChangesWhenUpperCased,

        [PropCode( "Changes_When_Titlecased", true )]
        ChangesWhenTitleCased,

        [PropCode( "Changes_When_Casefolded", true )]
        ChangesWhenCaseFolded,

        [PropCode( "Changes_When_Casemapped", true )]
        ChangesWhenCaseMapped,

        [PropCode( "ID_Start", true )]
        IdStart,

        [PropCode( "ID_Continue", true )]
        IdContinue,

        [PropCode( "XID_Start", true )]
        XidStart,

        [PropCode( "XID_Continue", true )]
        XidContinue,

        [PropCode( "Default_Ignorable_Code_Point", true )]
        DefaultIgnorableCodePoint,

        [PropCode( "Grapheme_Extend", true )]
        GraphemeExtend,

        [PropCode( "Grapheme_Base", true )]
        GraphemeBase,

        [PropCode( "Grapheme_Link", true )]
        GraphemeLink,
    }
}
