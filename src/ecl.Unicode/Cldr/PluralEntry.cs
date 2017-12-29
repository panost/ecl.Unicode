using System;
using System.Collections.Generic;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr {
    /// <summary>
    /// 3bits 0-7
    /// </summary>
    public enum PluralForm : byte {
        Default,
        Other,
        Zero,
        One,
        Two,
        Few,
        Many
    };
    /// <summary>
    /// 2bits
    /// </summary>
    public enum FormSize : byte {
        Normal,
        Small,
        Narrow = 3
    };

    public enum EntryType : byte {
        Normal,

        /// <summary>
        /// can be -2..2
        /// </summary>
        Relative,

        /// <summary>
        /// can be past or future
        /// </summary>
        RelativeTime,
    };

    

    

    
    public struct PluralEntry {
        public readonly string Text;
        public readonly PluralForm Type;
        public PluralEntry( PluralForm type, string text ) {
            Type = type;
            Text = text;
        }

        public static string TryParse( string text, out PluralForm count ) {
            const string Count = "count-";
            int idx = text.IndexOf( Count, StringComparison.OrdinalIgnoreCase );
            if ( idx >= 0 ) {
                int last = text.IndexOf( '-', idx + Count.Length );
                string ptext;
                if ( last >= 0 ) {
                    ptext = text.Substring( idx + Count.Length, last - idx + Count.Length );
                } else {
                    ptext = text.Substring( idx + Count.Length );
                }
                if ( Enum.TryParse( ptext, true, out count ) ) {
                    if ( last >= 0 ) {
                        ptext = text.Substring( last );
                    } else {
                        ptext = "";
                    }
                    if ( idx > 0 ) {
                        ptext = text.Substring( 0, idx ) + ptext;
                    }
                    return ptext;
                }
            }
            count = 0;
            return null;
        }
        internal static PluralEntry[] Load( IEnumerable<LdmlNode> nodes ) {
            var list = new List<PluralEntry>( 6 );
            foreach ( LdmlAnyNode elm in nodes ) {
                PluralForm form;
                if ( Enum.TryParse( elm.KeyValue, true, out form ) ) {
                    list.Add( new PluralEntry( form, elm.Value ) );
                }
            }
            return list.ToArray();
        }
        
    }

    
}
