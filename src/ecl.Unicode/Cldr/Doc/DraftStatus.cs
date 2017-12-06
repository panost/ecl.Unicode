using ecl.Unicode;

namespace eclUnicode.Cldr.Doc {
    [System.Flags]
    public enum DraftStatus : byte {
        None,
        Unconfirmed = 1,
        Provisional = 2,
        Contributed = 4,
        Approved = 8
    }
}

namespace eclUnicode.Cldr {
    using System;
    using eclUnicode.Cldr.Doc;

    public static partial class CldrUtil {

        //public static string ToCode( this DraftStatus status ) {
        //    if( status > 0 && status <= DraftStatus.Approved ) {
        //        return status.ToString().ToLowerInvariant();
        //    }
        //    return null;
        //}

        public static DraftStatus GetDraftStatus( string name ) {
            if( name.HasValue() ) {
                DraftStatus val;
                if ( Enum.TryParse( name, true, out val ) ) {
                    return val;
                }
            }
            return 0;
        }
    }
}