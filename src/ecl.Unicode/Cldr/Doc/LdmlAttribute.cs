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
        // flag attributes
        Draft=200
    };
}
