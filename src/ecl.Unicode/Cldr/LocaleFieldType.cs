namespace eclUnicode.Cldr {
    public enum LocaleFieldType : byte {
        Default,
        StandAlone,
    };

    partial class CldrUtil {
        public static string ToCode( this LocaleFieldType width ) {
            switch ( width ) {
            case LocaleFieldType.Default:
                return "format";
            case LocaleFieldType.StandAlone:
                return "stand-alone";
            }
            return null;
        }
    }
}
