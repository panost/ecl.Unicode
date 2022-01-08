using System;
using System.Collections.Generic;
using System.Text;

namespace ecl.Unicode.Cldr {
    /// <summary>
    /// Combination of genders in a list is treated as
    /// </summary>
    public enum GenderListType {
        /// <summary>
        /// Other
        /// </summary>
        Neutral,
        /// <summary>
        /// gender(all male) = male, gender(all female) = female, otherwise gender(list) = other
        /// </summary>
        MixedNeutral,
        /// <summary>
        /// gender(all female) = female, otherwise gender(list) = male
        /// </summary>
        MaleTaints
    }
}
