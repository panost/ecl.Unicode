using System.Diagnostics;
using ecl.Unicode.Cldr.Locale;

namespace ecl.Unicode.Cldr {
    [DebuggerDisplay( "{Code}:{Percent}" )]
    public struct LanguagePopulation {
        public LanguageOfficialStatus OfficialStatus;
        internal LanguageInfo Locale;
        public float Percent;

        public string Code {
            get {
                return Locale.Code;
            }
        }
    }
}
