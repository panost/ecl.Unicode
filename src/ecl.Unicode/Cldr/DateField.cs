using System;
using System.Collections.Generic;
using System.Globalization;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr {
    public enum DateFieldType : byte {
        Era = 1,
        Year,
        Month,
        Week,
        Day,
        WeekDay,
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        DayPeriod,
        Hour,
        Minute,
        Second,
        Zone,
    };

    public class DateField {
        public readonly DateFieldType Type;


        public DateField( DateFieldType tp ) {
            Type = tp;
            
        }
        
        private PluralEntry[] _future;
        /// <summary>
        /// 
        /// </summary>
        public PluralEntry[] Future {
            get {
                return _future;
            }
            set {
                _future = value;
            }
        }

        private PluralEntry[] _past;
        /// <summary>
        /// 
        /// </summary>
        public PluralEntry[] Past {
            get {
                return _past;
            }
            set {
                _past = value;
            }
        }

        private EnumKeyEntry[] _relativeEntries;
        public string this[ int idx ] {
            get {
                return _relativeEntries.GetText( idx );
            }
        }
        internal void Load( LdmlNode rootNode ) {
            Dictionary<short, string> relativeNames = new Dictionary<short, string>();

            foreach ( LdmlAnyNode elm in rootNode.Children ) {
                switch ( elm.Name ) {
                case "displayName":
                    _displayName = elm.Value;
                    break;
                case "relative":
                    short idx;
                    if ( short.TryParse( elm.KeyValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out idx ) ) {
                        relativeNames[ idx ] = elm.Value;
                    }
                    break;
                case "relativeTime":
                    var ent = PluralEntry.Load( elm.Children );
                    switch ( elm.KeyValue ) {
                    case "future":
                        _future = ent;
                        break;
                    case "past":
                        _past = ent;
                        break;
                    }
                    break;
                }
                
            }
            var relList = new List<EnumKeyEntry>( relativeNames.Count );
            foreach ( KeyValuePair<short, string> pair in relativeNames ) {
                relList.Add( new EnumKeyEntry( pair.Key, pair.Value ) );
            }
            relList.Sort();
            _relativeEntries = relList.ToArray();
        }
        



        private static readonly Dictionary<string, DateFieldType> _dateFieldMap = GetDateFieldMap();

        private static Dictionary<string, DateFieldType> GetDateFieldMap() {
            var map = new Dictionary<string, DateFieldType>( StringComparer.OrdinalIgnoreCase );
            map.Add( "era", DateFieldType.Era );
            map.Add( "year", DateFieldType.Year );
            map.Add( "month", DateFieldType.Month );
            map.Add( "week", DateFieldType.Week );
            map.Add( "day", DateFieldType.Day );
            map.Add( "weekday", DateFieldType.WeekDay );
            map.Add( "sun", DateFieldType.Sunday );
            map.Add( "mon", DateFieldType.Monday );
            map.Add( "tue", DateFieldType.Tuesday );
            map.Add( "wed", DateFieldType.Wednesday );
            map.Add( "thu", DateFieldType.Thursday );
            map.Add( "fri", DateFieldType.Friday );
            map.Add( "sat", DateFieldType.Saturday );
            map.Add( "dayperiod", DateFieldType.DayPeriod );
            map.Add( "hour", DateFieldType.Hour );
            map.Add( "minute", DateFieldType.Minute );
            map.Add( "second", DateFieldType.Second );
            map.Add( "zone", DateFieldType.Zone );
            map.Add( "Timezone", DateFieldType.Zone );
            map.Add( "Day-Of-Week", DateFieldType.WeekDay );
            return map;
        }

        public static DateFieldType ParseType( string val ) {
            if ( val.HasValue() ) {
                DateFieldType type;
                if ( _dateFieldMap.TryGetValue( val, out type ) ) {
                    return type;
                }
            }
            return 0;
        }
        public static int ParseWeekDay( string val ) {
            DateFieldType tp = ParseType( val );
            if ( tp >= DateFieldType.Sunday && tp <= DateFieldType.Saturday ) {
                return tp - DateFieldType.Sunday;
            }
            return -1;
        }
        private string _displayName;
        /// <summary>
        /// 
        /// </summary>
        public string DisplayName {
            get {
                return _displayName;
            }
            set {
                _displayName = value;
            }
        }

    }
}
