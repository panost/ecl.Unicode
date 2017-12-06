namespace eclUnicode.Ucd {
    public enum DecomposingTag:byte {
        /// <summary>
        /// &lt;font&gt;
        /// A font variant (e.g. a blackletter form).
        /// </summary>
        Font=1,

        /// <summary>
        /// &lt;noBreak&gt;
        ///  	A no-break version of a space or hyphen.
        /// </summary>
        NoBreak,

        /// <summary>
        /// &lt;initial&gt;
        ///  	An initial presentation form (Arabic).
        /// </summary>
        Initial,

        /// <summary>
        /// &lt;medial&gt;
        ///  	A medial presentation form (Arabic).
        /// </summary>
        Medial,

        /// <summary>
        /// &lt;final&gt;
        ///  	A final presentation form (Arabic).
        /// </summary>
        Final,

        /// <summary>
        /// &lt;isolated&gt;
        ///  	An isolated presentation form (Arabic).
        /// </summary>
        Isolated,

        /// <summary>
        /// &lt;circle&gt;
        ///  	An encircled form.
        /// </summary>
        Circle,

        /// <summary>
        /// &lt;super&gt;
        ///  	A superscript form.
        /// </summary>
        Super,

        /// <summary>
        /// &lt;sub&gt;
        ///  	A subscript form.
        /// </summary>
        Sub,

        /// <summary>
        /// &lt;vertical&gt;
        ///  	A vertical layout presentation form.
        /// </summary>
        Vertical,

        /// <summary>
        /// &lt;wide&gt;
        ///  	A wide (or zenkaku) compatibility character.
        /// </summary>
        Wide,

        /// <summary>
        /// &lt;narrow&gt;
        ///  	A narrow (or hankaku) compatibility character.
        /// </summary>
        Narrow,

        /// <summary>
        /// &lt;small&gt;
        ///  	A small variant form (CNS compatibility).
        /// </summary>
        Small,

        /// <summary>
        /// &lt;square&gt;
        ///  	A CJK squared font variant.
        /// </summary>
        Square,

        /// <summary>
        /// &lt;fraction&gt;
        ///  	A vulgar fraction form.
        /// </summary>
        Fraction,

        /// <summary>
        /// &lt;compat&gt;
        ///  	Otherwise unspecified compatibility character.
        /// </summary>
        Compat,
    };
}
