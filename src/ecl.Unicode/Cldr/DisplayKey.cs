using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr {
    public enum DisplayKey : byte {
        [KeyCode( "calendar" )]
        Calendar=1,

        [KeyCode( "colAlternate" )]
        IgnoreSymbolsSorting,

        [KeyCode( "colBackwards" )]
        ReversedAccentSorting,

        [KeyCode( "colCaseFirst" )]
        UppercaseLowercaseOrdering,

        [KeyCode( "colCaseLevel" )]
        CaseSensitiveSorting,

        [KeyCode( "colHiraganaQuaternary" )]
        KanaSorting,

        [KeyCode( "collation" )]
        SortOrder,

        [KeyCode( "colNormalization" )]
        NormalizedSorting,

        [KeyCode( "colNumeric" )]
        NumericSorting,

        [KeyCode( "colReorder" )]
        ScriptBlockReordering,

        [KeyCode( "colStrength" )]
        SortingStrength,

        [KeyCode( "currency" )]
        Currency,

        [KeyCode( "kv" )]
        HighestIgnored,

        [KeyCode( "lb" )]
        LineBreakStyle,

        [KeyCode( "numbers" )]
        Numbers,

        [KeyCode( "timezone" )]
        TimeZone,

        [KeyCode( "va" )]
        LocaleVariant,

        [KeyCode( "variableTop" )]
        SortAsSymbols,

        [KeyCode( "x" )]
        PrivateUse
    };
}
