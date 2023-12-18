namespace ecl.Unicode.Cldr.Doc {

    public enum LdmlAttribute : byte {
        None,

        Id = 10,
        Type,
        Request,
        NumberSystem,

        Count = 20,
        Ordinal,
        Subtype,
        Alt,
        Key,
        Scope,
        Level,
        Yeartype,

        Numbers,
        Sample,

        /// <summary>
        /// Gender, <see cref="GenderListType"/> and <see cref="GenderProperty"/>
        /// </summary>
        Gender,

        /// <summary>
        /// Case, <see cref="CaseProperty"/>
        /// </summary>
        Case,

        /// <summary>
        /// xml:space
        /// </summary>
        Space,
        Order,
        Length,
        Usage,
        Formality,
        Item,

		Parameter,
        // flag attributes
        Draft = 200,
    };
}
