namespace ecl.Unicode.Cldr {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// http://www.unicode.org/reports/tr35/tr35-dates.html#Date_Field_Symbol_Table
    /// http://cldr.unicode.org/translation/date-time
    /// </remarks>
    public enum DateTimeSymbol : byte {
        /// <summary>
        /// {1} {0}
        /// </summary>
        Argument=1,
        Era,
        Year,
        YearOfWeeks,
        ExtendedYear,
        CyclicYear,
        RelatedYear,
        Quarter,
        QuarterStandAlone,
        Month,
        MonthStandAlone,
        WeekOfYear,
        WeekOfMonth,
        DayOfMonth,
        DayOfYear,
        DayOfWeek,
        JulianDay,
        WeekDay,
        WeekDayLocal,
        WeekDayLocalStandAlone,
        Period,
        Hour12,
        Hour24,
        SkeletonHour12,
        SkeletonHour24,
        Minute,
        Second,
        SecondFractional,
        Milliseconds,
        TimeSeparator,
        Zone,
        ZoneISO8601,
        ZoneGMT,
        ZoneGeneric,
        ZoneId,
        ZoneBasic,
        ZoneBasicZ,

    }
}
