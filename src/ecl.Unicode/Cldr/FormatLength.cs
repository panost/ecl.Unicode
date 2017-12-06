namespace eclUnicode.Cldr {
    /// <summary>
    /// Standard format size
    /// </summary>
    public enum FormatLength : byte {
        Full,
        Long,
        Medium,
        Short,
        Narrow,
    }

    partial class CldrUtil {
        public static string ToCode( this FormatLength width ) {
            switch ( width ) {
            case FormatLength.Full:
                return "full";
            case FormatLength.Long:
                return "long";
            case FormatLength.Medium:
                return "medium";
            case FormatLength.Short:
                return "short";
            case FormatLength.Narrow:
                return "narrow";
            }
            return null;
        }
    }
}
