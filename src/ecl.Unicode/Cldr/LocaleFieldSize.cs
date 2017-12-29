namespace ecl.Unicode.Cldr {
    public enum LocaleFieldSize : byte {
        Narrow,
        Short,
        Abbreviated,
        Wide,
    }

    partial class CldrUtil {
        public static string ToCode( this LocaleFieldSize width ) {
            switch ( width ) {
            case LocaleFieldSize.Narrow:
                return "narrow";
            case LocaleFieldSize.Short:
                return "short";
            case LocaleFieldSize.Abbreviated:
                return "abbreviated";
            case LocaleFieldSize.Wide:
                return "wide";
            }
            return null;
        }
    }
}
