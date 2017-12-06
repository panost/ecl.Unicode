namespace eclUnicode.Cldr.Doc {
    public enum LdmlAttribute : byte {
        None,

        Id = 10,
        Type,
        Request,
        NumberSystem,

        Count = 20,
        Alt,
        Key,
        Yeartype,

        Numbers,
        // flag attributes
        Draft=200
    };
}
