namespace eclUnicode.Cldr {
    public enum DisplayNameType : byte {
        Calendar = 1,
        Collation,
        Numbers,
        ColAlternate,
        ColBackwards,
        colCaseFirst,
        colCaseLevel,
        colHiraganaQuaternary,
        colNormalization,
        colNumeric,
        colStrength,
        va
    };

    public enum DisplayNameTypeOption : ushort {
        Calendar = DisplayNameType.Calendar << 8,
        Collation = DisplayNameType.Collation << 8,
        Numbers = DisplayNameType.Numbers << 8,
        ColAlternate = DisplayNameType.ColAlternate << 8,
        ColBackwards = DisplayNameType.ColBackwards << 8,
        colCaseFirst = DisplayNameType.colCaseFirst << 8,
        colCaseLevel = DisplayNameType.colCaseLevel << 8,
        colHiraganaQuaternary = DisplayNameType.colHiraganaQuaternary << 8,
        colNormalization = DisplayNameType.colNormalization << 8,
        colNumeric = DisplayNameType.colNumeric << 8,
        colStrength = DisplayNameType.colStrength << 8,
        va = DisplayNameType.va << 8,


    };
}
