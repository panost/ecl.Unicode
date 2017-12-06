using System;
using System.Diagnostics;
using eclUnicode.Cldr.Locale;

namespace eclUnicode.Cldr {
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
