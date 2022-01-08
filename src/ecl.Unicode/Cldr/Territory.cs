using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using ecl.Unicode.Cldr.Doc;

namespace ecl.Unicode.Cldr {
    [Flags]
    public enum TerritoryTypes : byte {
        Group = 1,
        /// <summary>
        /// Territory contains others, hierarchy, group or subdivisions
        /// </summary>
        Containment = 2,
        Deprecated = 4,

        //Code = 8,
        MetaGroup = 16,
        Obsolete = 32,

        /// <summary>
        /// Subdivision Containment
        /// </summary>
        /// <cite>
        /// https://unicode-org.github.io/cldr/ldml/tr35-info.html#Subdivision_Containment
        /// https://en.wikipedia.org/wiki/ISO_3166-2
        /// </cite>
        Subdivisioned = 64
    }

    [System.Diagnostics.DebuggerDisplay( "Code = {_code}, {GetDebugHint()}" )]
    public class Territory : CodeObjectBase {
        private short _id;

        /// <summary>
        /// 
        /// </summary>
        public short Id {
            get {
                if ( _id == 0 && _code.HasValue() ) {
                    if( !short.TryParse( _code, NumberStyles.Integer, CultureInfo.InvariantCulture, out _id ) ) {
                        _id = -3;
                    }
                }
                return _id;
            }
            //set {
            //    _id = value;
            //}
        }
        private TerritoryTypes _type;
        /// <summary>
        /// 
        /// </summary>
        public TerritoryTypes Type {
            get {
                return _type;
            }
            set {
                _type = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Code {
            get {
                return _code;
            }
            set {
                _code = value;
            }
        }

        private Territory _container;

        /// <summary>
        /// 
        /// </summary>
        public Territory Container {
            get {
                return _container;
            }
            internal set {
                _container = value;
            }
        }

        public string Alpha3 { get; set; }

        public string Fips10 { get; set; }

        public string[] Internet { get; set; }

        public Territory() {

        }

        public Territory(string code) {
            _code = code;
        }
        
        internal void LoadCodes( List<AttributeValue> list ) {
            foreach ( AttributeValue value in list ) {
                switch ( value.Name ) {
                //case "type":
                //    _code = value.Value;
                //    break;
                case "numeric":
                    if ( !short.TryParse( value.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _id ) ) {
                        _id = -2;
                    }
                    break;
                case "alpha3":
                    Alpha3 = value.Value;
                    break;
                case "fips10":
                    Fips10 = value.Value;
                    break;
                case "internet":
                    Internet = value.Value.SplitAtSpaces();
                    break;
                }
            }
        }

        
        private string _description;
        /// <summary>
        /// 
        /// </summary>
        public string Description {
            get {
                return _description;
            }
            internal set {
                _description = value;
            }
        }

        private Territory[] _contains;


        /// <summary>
        /// 
        /// </summary>
        public Territory[] Contains {
            get {
                return _contains;
            }
            internal set {
                _contains = value;
            }
        }
        protected virtual string GetDebugHint() {
            var str = _id.ToString();
            if ( _type != 0 ) {
                str += " type:" + _type;
            }
            if ( _container != null ) {
                str += " parent:" + _container._code;
            }
            return str;
        }

        public override string ToString() {
            var str = _code;
            if ( _type != 0 ) {
                str += " :" + _type;
            }
            return str;
        }

        public DateRangeValue<Currency>[] Currencies {
            get;
            set;
        }

        public string AlternateEnglishName {
            get;
            set;
        }

        #region Week Data
        private byte _weekMinDays;
        /// <summary>
        /// Minimum number of days of the first week of a year
        /// </summary>
        public byte WeekMinDays {
            get {
                if ( _weekMinDays == 0 && _container != null ) {
                    _weekMinDays = _container.WeekMinDays;
                }
                return _weekMinDays;
            }
            set { _weekMinDays = value; }
        }

        private byte _weekFirstDay=0xFF;
        /// <summary>
        /// first day of the week in a calendar view
        /// </summary>
        public DayOfWeek WeekFirstDay {
            get {
                if ( _weekFirstDay == 0xFF && _container != null ) {
                    _weekFirstDay = (byte)_container.WeekFirstDay;
                }
                return ( DayOfWeek)_weekFirstDay;
            }
            set { _weekFirstDay = (byte)value; }
        }

        private byte _weekendStart = 0xFF;
        /// <summary>
        /// 
        /// </summary>
        public DayOfWeek WeekendStart {
            get {
                if( _weekendStart == 0xFF && _container != null ) {
                    _weekendStart = (byte)_container.WeekendStart;
                }
                return (DayOfWeek)_weekendStart;
            }
            set { _weekendStart = (byte)value; }
        }

        private byte _weekendEnd = 0xFF;
        /// <summary>
        /// 
        /// </summary>
        public DayOfWeek WeekendEnd {
            get {
                if( _weekendEnd == 0xFF && _container != null ) {
                    _weekendEnd = (byte)_container.WeekendEnd;
                }
                return (DayOfWeek)_weekendEnd;
            }
            set { _weekendEnd = (byte)value; }
        }

        #endregion

        private long _gdp;
        /// <summary>
        /// 
        /// </summary>
        public long Gdp {
            get { return _gdp; }
            set { _gdp = value; }
        }
        private long _population;
        /// <summary>
        /// 
        /// </summary>
        public long Population {
            get { return _population; }
            set { _population = value; }
        }

        internal bool LoadInfo( CldrLoader loader, XmlReader reader ) {
            var text = reader.GetAttribute( "gdp" );
            if ( text.HasValue() ) {
                long.TryParse( text, NumberStyles.Integer, CultureInfo.InvariantCulture, out _gdp );
            }
            text = reader.GetAttribute( "population" );
            if( text.HasValue() ) {
                long.TryParse( text, NumberStyles.Integer, CultureInfo.InvariantCulture, out _population );
            }
            reader.LoadElements( loader, LanguagePopulation );
            return true;
        }
        private readonly List<LanguagePopulation> _languages = new List<LanguagePopulation>();
        /// <summary>
        /// 
        /// </summary>
        public List<LanguagePopulation> Languages {
            get { return _languages; }
        }

        private bool LanguagePopulation( XmlReader reader, CldrLoader loader ) {
            if( reader.Name == "languagePopulation" ) {

                var type = reader.GetAttribute( "type" );
                float population;
                var perc = reader.GetAttribute( "populationPercent" );
                if ( perc.HasValue() &&
                     float.TryParse( perc, NumberStyles.Float, CultureInfo.InvariantCulture, out population ) ) {
                    LanguagePopulation lang = new LanguagePopulation();
                    lang.Locale = loader.GetLocaleInfo( type );
                    lang.Percent = population;
                    var sts = reader.GetAttribute( "officialStatus" );
                    if ( sts.HasValue() ) {
                        switch ( sts ) {
                        case "official":
                            lang.OfficialStatus = LanguageOfficialStatus.Official;
                            break;
                        case "official_regional":
                            lang.OfficialStatus = LanguageOfficialStatus.OfficialRegional;
                            break;
                        case "de_facto_official":
                            lang.OfficialStatus = LanguageOfficialStatus.DeFactoOfficial;
                            break;
                        default:
                            lang.OfficialStatus = (LanguageOfficialStatus)5;
                            break;
                        }
                    }
                    _languages.Add( lang );
                }
            }
            return false;
        }
    }

}