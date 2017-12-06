namespace eclUnicode.Ucd {
    /// <summary>
    /// 
    /// </summary>
    public enum CodePointProperty : byte {
        None,
        /// <summary>
        /// ASCII characters commonly used for the representation of hexadecimal numbers.
        /// </summary>
        ASCII_Hex_Digit,
        /// <summary> Format control characters which have specific functions in the Unicode Bidirectional Algorithm [UAX9].</summary>
        Bidi_Control,
        /// <summary> Punctuation characters explicitly called out as dashes in the Unicode Standard, plus their compatibility equivalents. Most of these have the General_Category value Pd, but some have the General_Category value Sm because of their use in mathematics.</summary>
        Dash,
        /// <summary>
        /// For a machine-readable list of deprecated characters.
        /// No characters will ever be removed from the standard,
        /// but the usage of deprecated characters is strongly discouraged.
        /// </summary>
        Deprecated,
        /// <summary> Characters that linguistically modify the meaning of another character to which they apply. Some diacritics are not combining characters, and some combining characters are not diacritics.</summary>
        Diacritic,
        /// <summary> Characters whose principal function is to extend the value or shape of a preceding alphabetic character. Typical of these are length and iteration marks.</summary>
        Extender,
        /// <summary>
        /// Characters commonly used for the representation of hexadecimal numbers, 
        /// plus their compatibility equivalents.
        /// </summary>
        Hex_Digit,
        /// <summary> of 4.0.0; Deprecated as of 6.0.0  B I Dashes which are used to mark connections between pieces of words, plus the Katakana middle dot. The Katakana middle dot functions like a hyphen, but is shaped like a dot rather than a dash.</summary>
        Hyphen,
        /// <summary> Characters considered to be CJKV (Chinese, Japanese, Korean, and Vietnamese) ideographs. This property roughly defines the class of "Chinese characters" and does not include characters of other logographic scripts such as Cuneiform or Egyptian Hieroglyphs.</summary>
        Ideographic,
        /// <summary> Used in Ideographic Description Sequences.</summary>
        IDS_Binary_Operator,
        /// <summary> Used in Ideographic Description Sequences.</summary>
        IDS_Trinary_Operator,
        /// <summary> Format control characters which have specific functions for control of cursive joining and ligation.</summary>
        Join_Control,
        /// <summary> A small number of spacing vowel letters occurring in certain Southeast Asian scripts such as Thai and Lao, which use a visual order display model. These letters are stored in text ahead of syllable-initial consonants, and require special handling for processes such as searching and sorting.</summary>
        Logical_Order_Exception,
        /// <summary> Code points permanently reserved for internal use.</summary>
        Noncharacter_Code_Point,
        /// <summary> Used in deriving the Alphabetic property.</summary>
        Other_Alphabetic,
        /// <summary> Used in deriving the Default_Ignorable_Code_Point property.</summary>
        Other_Default_Ignorable_Code_Point,
        /// <summary> Used in deriving  the Grapheme_Extend property.</summary>
        Other_Grapheme_Extend,
        /// <summary> Used to maintain backward compatibility of ID_Continue.</summary>
        Other_ID_Continue,
        /// <summary> Used to maintain backward compatibility of ID_Start.</summary>
        Other_ID_Start,
        /// <summary> Used in deriving the Lowercase property.</summary>
        Other_Lowercase,
        /// <summary> Used in deriving the Math property.</summary>
        Other_Math,
        /// <summary> Used in deriving the Uppercase property.</summary>
        Other_Uppercase,
        /// <summary> Used for pattern syntax as described in Unicode Standard Annex #31, "Unicode Identifier and Pattern Syntax" [UAX31].</summary>
        Pattern_Syntax,
        /// <summary> Punctuation characters that function as quotation marks.</summary>
        Quotation_Mark,
        /// <summary> Used in Ideographic Description Sequences.</summary>
        Radical,
        /// <summary> Characters with a "soft dot", like i or j. An accent placed on these characters causes the dot to disappear. An explicit dot above can be added where required, such as in Lithuanian.</summary>
        Soft_Dotted,
        /// <summary> Sentence Terminal. Used in Unicode Standard Annex #29, "Unicode Text Segmentation" [UAX29].</summary>
        STerm,
        /// <summary> Punctuation characters that generally mark the end of textual units.</summary>
        Terminal_Punctuation,
        /// <summary> A property which specifies the exact set of Unified CJK Ideographs in the standard. This set excludes CJK Compatibility Ideographs (which have canonical decompositions to Unified CJK Ideographs), as well as characters from the CJK Symbols and Punctuation block. The property is used in the definition of Ideographic Description Sequences.</summary>
        Unified_Ideograph,
        /// <summary> Indicates characters that are Variation Selectors. For details on the behavior of these characters, see StandardizedVariants.html, Section 23.4, Variation Selectors in [Unicode], and Unicode Technical Standard #37, "Unicode Ideographic Variation Database" [UTS37].</summary>
        Variation_Selector,
        /// <summary> Spaces, separator characters and other control characters which should be treated by programming languages as "white space" for the purpose of parsing elements. See also Line_Break, Grapheme_Cluster_Break, Sentence_Break, and Word_Break, which classify space characters and related controls somewhat differently for particular text segmentation contexts.</summary>
        White_Space,

        MaxValue
    }
}
