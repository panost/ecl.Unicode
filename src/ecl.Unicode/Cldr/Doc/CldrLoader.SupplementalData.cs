using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using ecl.Unicode.Cldr.Locale;

namespace ecl.Unicode.Cldr.Doc {
    partial class CldrLoader {
        private readonly Dictionary<string, LanguageInfo> _localeInfo =
            new Dictionary<string, LanguageInfo>( StringComparer.OrdinalIgnoreCase );

        internal CldrLocale GetLocaleParent( string code ) {
            int idx = code.IndexOf( '_' );
            if ( idx > 0 ) {
                code = code.Substring( 0, idx );
                return GetLocale( code );
            }
            return null;
        }
        internal LanguageInfo FindLanguage( ref string code ) {
            if( !code.HasValue() )
                return null;
            if( code.IndexOf( '-' ) >= 0 ) {
                code = code.Replace( '-', '_' );
            }
            AliasInfo alias;
            if( _languageAliases.TryGetValue( code, out alias ) ) {
                if( alias.Many ) {
                    throw new Exception( $"Many replacements for {code}:'{alias.Replacement}'" );
                }
                code = alias.Replacement;
            }
            return _localeInfo.GetValueOrDefault( code );
        }
        internal LanguageInfo GetLocaleInfo( string code ) {
            var info = FindLanguage( ref code );
            if( info == null ) {
                info = new LanguageInfo() {
                    Code = code
                };
                _localeInfo.Add( code, info );
            }
            return info;
        }

        //internal LanguageInfo FindLanguage( string code ) {
        //    return FindLanguage( ref code );
        //}

        internal LanguageInfo ResolveLanguage( string code ) {
            var info = FindLanguage( ref code );
            if ( info == null ) {
                throw new ArgumentException( $"Unknwon language '{code}'", nameof(code) );
            }
            return info;
        }
        internal readonly Dictionary<string, Territory> _territories = new Dictionary<string, Territory>( StringComparer.OrdinalIgnoreCase );
        //private Territory GetOrCreate( string territoryType ) {
        //    Territory territory;
        //    if ( !_territories.TryGetValue( territoryType, out territory ) ) {
        //        territory = new Territory();
        //    }
        //    _territories.Add( territory.Code, territory );
        //}

        public Dictionary<string, Territory>.ValueCollection Territories {
            get {
                return _territories.Values;
            }
        }

        public Currency FindCurrency( string code ) {
            if ( _currencies.TryGetValue( code, out var value ) ) {
                return value;
            }

            return null;
        }

        public IReadOnlyCollection<Currency> GetCurrencies() {
            return _currencies.Values;
        }
        private readonly Dictionary<string, Currency> _currencies = new Dictionary<string, Currency>( StringComparer.OrdinalIgnoreCase );
        
        private void Add( Currency currency ) {
            _currencies.Add( currency.Code, currency );
        }

        struct CalendarPreference {
            public string[] Territories;
            public string[] Calendars;
        }

        partial class MetaDataLoader {
            //internal readonly Dictionary<string, Territory> _territories = new Dictionary<string, Territory>( StringComparer.OrdinalIgnoreCase );
            private void LoadCodeMappings( string elmName, List<AttributeValue> list ) {
                switch ( elmName ) {
                case "territoryCodes":
                    var t = _loader._territories.GetOrCreate( list.GetValue( "type" ) );
                    t.LoadCodes( list );
                    //_loader.Add( new Territory( list ) );
                    break;
                case "currencyCodes":
                    _loader.Add( new Currency( list ) );
                    break;
                default:
                    _loader.Warning( "Invalid element " + elmName );
                    break;
                }
            }
            List<CalendarPreference> _calendarPreferences = new List<CalendarPreference>();

            [DebuggerDisplay("{Code}:{tp}")]
            struct TerContains {
                public string Code;
                public string contains;
                public TerritoryTypes tp;
            }
            List<TerContains> _terContains = new List<TerContains>();

            private void LoadSubdivisionContainment( string elmName, List<AttributeValue> list ) {
                if ( elmName != "subgroup" ) {
                    return;
                }
                TerContains ter = new TerContains();
                foreach ( AttributeValue value in list ) {
                    switch ( value.Name ) {
                    case "type":
                        ter.Code = value.Value;
                        break;
                    case "contains":
                        ter.contains = value.Value;
                        break;
                    default:
                        _loader.Warning( "Invalid attribute " + value.Name );
                        break;
                    }
                }
                if ( ter.Code.HasValue() && ter.contains.HasValue() ) {
                    ter.tp = TerritoryTypes.Subdivisioned;
                    _terContains.Add( ter );
                }
            }
            private void LoadTerritoryContainment( string elmName, List<AttributeValue> list ) {
                if ( elmName != "group" ) {
                    return;
                }
                TerContains ter = new TerContains();
                foreach ( AttributeValue value in list ) {
                    switch ( value.Name ) {
                    case "type":
                        ter.Code = value.Value;
                        break;
                    case "contains":
                        ter.contains = value.Value;
                        break;
                    case "grouping":
                        if ( value.Value == "true" ) {
                            ter.tp |= TerritoryTypes.Group;
                        }
                        break;
                    case "status":
                        switch ( value.Value ) {
                        case "deprecated":
                            ter.tp |= TerritoryTypes.Deprecated;
                            break;
                        case "grouping":
                            ter.tp |= TerritoryTypes.MetaGroup;
                            break;
                        }
                        break;
                    default:
                        _loader.Warning( "Invalid attribute " + value.Name );
                        break;
                    }
                }
                if ( ter.Code.HasValue() && ter.contains.HasValue() ) {
                    _terContains.Add( ter );
                }
            }
            private void LoadLanguageData( string elmName, List<AttributeValue> list ) {
                if( elmName != "language" ) {
                    return;
                }
                LanguageInfo locale = null;
                string scripts = null;
                string territories = null;
                bool secondary = false;
                foreach( AttributeValue value in list ) {
                    switch( value.Name ) {
                    case "type":
                        locale = _loader.GetLocaleInfo(value.Value);
                        break;
                    case "scripts":
                        scripts = value.Value;
                        break;
                    case "territories":
                        territories = value.Value;
                        break;
                    case "alt":
                        if ( value.Value == "secondary" ) {
                            secondary = true;
                        }
                        break;
                    default:
                        _loader.Warning( "Invalid attribute " + value.Name );
                        break;
                    }
                }
                if( locale != null ) {
                    if ( scripts.HasValue() ) {
                        var als = scripts.SplitAtSpaces();
                        WritingScript[] s = new WritingScript[ als.Length ];
                        for( int i = 0; i < s.Length; i++ ) {
                            s[ i ] = _loader.GetScript( als[ i ] );
                        }
                        locale.AddScripts( s, secondary );
                    }
                    if ( territories.HasValue() ) {
                        locale.AddTerritories( territories.SplitAtSpaces(_loader._territories), secondary );
                    }
                }
            }
            private void LoadParentLocale( string elmName, List<AttributeValue> list ) {
                LanguageInfo parent = null;
                string locales = null;
                foreach ( AttributeValue value in list ) {
                    switch ( value.Name ) {
                    case "parent":
                        parent = _loader.GetLocaleInfo(value.Value);
                        break;
                    case "locales":
                        locales = value.Value;
                        break;
                    default:
                        _loader.Warning( "Invalid attribute " + value.Name );
                        break;
                    }
                }
                //var map = _loader._parentLocales;
                if ( parent!=null && locales.HasValue() ) {
                    foreach ( string locale in locales.SplitAtSpaces() ) {
                        _loader.GetLocaleInfo( locale ).Parent = parent;
                    }
                }
            }

            //struct CurrencyRange {
            //    public string Iso4217;
            //    public DateTime From;
            //    public DateTime To;
            //}
            class CurrencyRegion {
                private string _iso3166;
                /// <summary>
                /// 
                /// </summary>
                public string Iso3166 {
                    get {
                        return _iso3166;
                    }
                    set {
                        _iso3166 = value;
                    }
                }
                private readonly List<DateRangeValue<string>> _ranges = new List<DateRangeValue<string>>();
                /// <summary>
                /// 
                /// </summary>
                public List<DateRangeValue<string>> Ranges {
                    get {
                        return _ranges;
                    }
                }

                void Load( string elmName, List<AttributeValue> list ) {
                    DateRangeValue<string> range = new DateRangeValue<string>();
                    foreach ( AttributeValue value in list ) {
                        switch ( value.Name ) {
                        case "iso4217":
                            range.Value = value.Value;
                            break;
                        case "from":
                            DateTime.TryParseExact( value.Value, "yyyy-MM-dd", 
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AdjustToUniversal, out range.From );
                            break;
                        case "to":
                            DateTime.TryParseExact( value.Value, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AdjustToUniversal, out range.To );
                            break;
                        }
                    }
                    if ( range.Value.HasValue() && range.From > default(DateTime) ) {
                        _ranges.Add( range );
                    }
                }
                
                public CurrencyRegion( XmlReader reader ) {
                    if ( reader.MoveToFirstAttribute() ) {
                        while ( true ) {
                            if ( reader.Name == "iso3166" ) {
                                _iso3166 = reader.Value;
                                break;
                            }
                            //list.Add( new AttributeValue( reader.Name, reader.Value ) );
                            if ( !reader.MoveToNextAttribute() ) {
                                break;
                            }
                        }
                        reader.MoveToElement();
                    }
                    reader.LoadNodes( Load );

                }
            }
            List<CurrencyRegion> _currencyRegions = new List<CurrencyRegion>();

            struct FractionInfo {
                public byte Digits;
                public byte Rounding;
                public byte CashRounding;
                public byte CashDigits;
            }

            private readonly Dictionary<string, FractionInfo> _fractionInfos =
                new Dictionary<string, FractionInfo>( StringComparer.OrdinalIgnoreCase );

            private void LoadFractions( string elmName, List<AttributeValue> list ) {
                FractionInfo f = new FractionInfo();
                string code = null;
                foreach ( AttributeValue value in list ) {
                    switch ( value.Name ) {
                    case "iso4217":
                        code = value.Value;
                        if ( code == "DEFAULT" ) {
                            return;
                        }
                        break;
                    case "digits":
                        byte.TryParse( value.Value, NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out f.Digits );
                        break;
                    case "rounding":
                        byte.TryParse( value.Value, NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out f.Rounding );
                        break;
                    case "cashDigits":
                        byte.TryParse( value.Value, NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out f.CashDigits );
                        break;
                    case "cashRounding":
                        byte.TryParse( value.Value, NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out f.CashRounding );
                        break;
                    default:
                        _loader.Warning( "Invalid attribute " + value.Name );
                        break;
                    }
                }
                if ( code.HasValue() ) {
                    _fractionInfos.Add( code, f );
                }
            }


            private void LoadCurrencyData( XmlReader reader ) {
                while ( reader.Read() ) {
                    var type = reader.NodeType;
                    if ( type == XmlNodeType.Element ) {
                        if ( !reader.IsEmptyElement ) {
                            switch ( reader.Name ) {
                            case "fractions":
                                reader.LoadNodes( LoadFractions );
                                break;
                            case "region":
                                _currencyRegions.Add( new CurrencyRegion( reader ) );
                                break;
                            default:
                                reader.SkipElement();
                                break;
                            }
                        }
                    } else if ( type == XmlNodeType.EndElement ) {
                        break;
                    }
                }
            }

            public void LoadSubdivisions(XmlReader reader) {
                reader.MoveToContent();
                while ( reader.Read() ) {
                    var type = reader.NodeType;
                    if ( type == XmlNodeType.Element ) {
                        if ( !reader.IsEmptyElement ) {
                            switch ( reader.Name ) {
                            case "subdivisionContainment":
                                reader.LoadNodes( LoadSubdivisionContainment );
                                break;
                            default:
                                reader.SkipElement();
                                break;
                            }
                        }
                    } else if ( type == XmlNodeType.EndElement ) {
                        break;
                    }
                }
            }
            /// <summary>
            /// load supplemental Data
            /// </summary>
            /// <param name="reader"></param>
            public void Load( XmlReader reader ) {
                reader.MoveToContent();
                while ( reader.Read() ) {
                    var type = reader.NodeType;
                    if ( type == XmlNodeType.Element ) {
                        if ( !reader.IsEmptyElement ) {
                            switch ( reader.Name ) {
                            case "currencyData":
                                LoadCurrencyData( reader );
                                break;
                            case "territoryInfo":
                                reader.LoadElements( LoadTerritoryInfo );
                                break;
                            case "territoryContainment":
                                reader.LoadNodes( LoadTerritoryContainment );
                                break;
                            case "languageData":
                                reader.LoadNodes( LoadLanguageData );
                                break;
                            case "parentLocales":
                                reader.LoadNodes( LoadParentLocale );
                                break;
                            case "codeMappings":
                                reader.LoadNodes( LoadCodeMappings );
                                break;
                            case "calendarData":
                                reader.LoadNodes( LoadCalendarData );
                                break;
                            case "calendarPreferenceData":
                                reader.LoadNodes( LoadCalendarPreferenceData );
                                break;
                            case "weekData":
                                reader.LoadNodes( LoadWeekData );
                                break;
                            default:
                                reader.SkipElement();
                                break;
                            }
                        }
                    } else if ( type == XmlNodeType.EndElement ) {
                        break;
                    }
                }
                CombineTerritories();
                CombineCurrencyRegions();
                CombineCurrencyFractions();
            }
            /// <summary>
            /// Supplemental Territory Information
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            /// <remarks>
            /// http://unicode.org/reports/tr35/tr35-info.html#Supplemental_Territory_Information
            /// </remarks>
            private bool LoadTerritoryInfo( XmlReader reader ) {
                if ( reader.Name == "territory" ) {
                    var type = reader.GetAttribute( "type" );
                    
                    var ter = _loader._territories.GetOrCreate( type );
                    return ter.LoadInfo( _loader, reader );
                }
                reader.SkipElement();
                return true;
            }


            private void LoadCalendarData( string elmName, List<AttributeValue> list ) {
                if ( elmName == "calendar" && list.Count > 0 ) {
                    if ( list[ 0 ].Name == "type" ) {
                        Calendar c = this._loader.GetOrCreateCalendar( list[ 0 ].Value );

                    }
                }
                
            }
            private void LoadCalendarPreferenceData( string elmName, List<AttributeValue> list ) {
                if( elmName != "calendarPreference" )
                    return;
                string territories = null;
                string[] calendars = null;
                foreach( AttributeValue value in list ) {
                    switch( value.Name ) {
                    case "territories":
                        territories = value.Value;
                        break;
                    case "ordering":
                        calendars = value.Value.SplitAtSpaces();
                        break;
                    }
                }
                if( territories.HasValue() && calendars != null && calendars.Length > 0 ) {
                    var calendarList = new List<Calendar>( calendars.Length );
                    for( int i = 0; i < calendars.Length; i++ ) {
                        var c = this._loader.GetCalendar( calendars[ i ] );
                        if( !calendarList.Contains( c ) ) {
                            calendarList.Add( c );
                        }
                    }
                    var cals = calendarList.ToArray();
                    if( territories == "001" ) {
                        this._loader.DefaultCalendars = cals;
                    } else {
                        foreach( var s in territories.SplitAtSpaces() ) {
                            this._loader._calendarPreferences.Add( s, cals );
                        }
                    }
                } else {
                    _loader.Warning( "Invalid element " + elmName );
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="elmName"></param>
            /// <param name="list"></param>
            /// <remarks>
            /// http://www.unicode.org/reports/tr35/tr35-dates.html#Week_Data
            /// </remarks>
            private void LoadWeekData( string elmName, List<AttributeValue> list ) {
                List<Territory> territories = null;
                string count = null;
                var day = -1;
                string alt = null;
                foreach( AttributeValue value in list ) {
                    switch( value.Name ) {
                    case "territories":
                        territories = new List<Territory>();
                        foreach ( var terCode in value.Value.SplitAtSpaces() ) {
                            territories.Add( _loader._territories.GetOrCreate( terCode ) );
                        }
                        break;
                    case "alt":
                        alt= value.Value;
                        break;
                    case "count":
                        count = value.Value;
                        break;
                    case "day":
                        day = DateField.ParseWeekDay( value.Value );
                        break;
                    }
                }
                switch ( elmName ) {
                case "minDays":
                    byte minDays;
                    if ( territories != null &&
                         count.HasValue() &&
                         byte.TryParse( count, NumberStyles.Integer, CultureInfo.InvariantCulture, out minDays ) ) {
                        foreach ( Territory territory in territories ) {
                            territory.WeekMinDays = minDays;
                        }
                    }
                    break;
                case "firstDay":
                    if( territories != null && day >= 0 && !alt.HasValue()) {
                        foreach( Territory territory in territories ) {
                            territory.WeekFirstDay = (DayOfWeek)day;
                        }
                    }
                    break;
                case "weekendStart":
                    if( territories != null && day >= 0 ) {
                        foreach( Territory territory in territories ) {
                            territory.WeekendStart = (DayOfWeek)day;
                        }
                    }
                    break;
                case "weekendEnd":
                    if( territories != null && day >= 0 ) {
                        foreach( Territory territory in territories ) {
                            territory.WeekendEnd = (DayOfWeek)day;
                        }
                    }
                    break;
                default:
                    return;
                }
                
            }


            private void CombineCurrencyFractions() {
                var currencies = _loader._currencies;

                foreach ( KeyValuePair<string, FractionInfo> pair in _fractionInfos ) {
                    Currency cur;
                    if ( !currencies.TryGetValue( pair.Key, out cur ) ) {
                        //_loader.Warning( "Invalid Currency " + pair.Key );
                        continue;
                    }

                    cur.Digits = pair.Value.Digits;
                    cur.Rounding = pair.Value.Rounding;
                    cur.CashRounding = pair.Value.CashRounding;
                    cur.CashDigits = pair.Value.CashDigits;
                }
            }

            private void CombineCurrencyRegions() {
                var territories = _loader._territories;
                var currencies = _loader._currencies;
                var curMap = new Dictionary<Currency, List<DateRangeValue<Territory>>>();
                var list = new List<DateRangeValue<Currency>>();

                foreach ( CurrencyRegion region in _currencyRegions ) {
                    Territory ter;
                    if ( !territories.TryGetValue( region.Iso3166, out ter ) ) {
                        _loader.Warning( "Invalid territory " + region.Iso3166 );
                        continue;
                    }
                    list.Clear();
                    foreach ( DateRangeValue<string> range in region.Ranges ) {
                        if ( !currencies.TryGetValue( range.Value, out Currency cur ) ) {
                            cur = new Currency {
                                Code = range.Value,
                                Deprecated = true
                            };
                            currencies.Add( cur.Code, cur );
                            //Debug.WriteLine( "Currency added:" + range.Value );
                        }

                        list.Add( new DateRangeValue<Currency>() {
                            From = range.From,
                            To = range.To,
                            Value = cur
                        } );
                        curMap.AddGroup( cur, new DateRangeValue<Territory>() {
                            From = range.From,
                            To = range.To,
                            Value = ter
                        });
                    }
                    ter.Currencies = list.ToArray();
                }
                foreach ( var pair in curMap ) {
                    pair.Key.Territories = pair.Value.ToArray();
                }
            }

            private void CombineTerritories() {
                var children = new Dictionary<Territory, List<Territory>>();
                var territories = _loader._territories;
                foreach ( TerContains grp in _terContains ) {
                    Territory ter = territories.GetOrCreate( grp.Code );
                    ter.Type |= TerritoryTypes.Containment;
                    ter.Type |= grp.tp & ( TerritoryTypes.Group | TerritoryTypes.Subdivisioned );

                    foreach ( string child in grp.contains.SplitAtSpaces() ) {
                        Territory terChild=territories.GetOrCreate( child );
                        children.AddGroup( ter, terChild );
                        if ( ( grp.tp & TerritoryTypes.Group ) == 0 ) {
                            if ( terChild.Container != null ) {
                                _loader.Warning( "Territory has Container " + terChild.Container.Code );
                            } else {
                                terChild.Container = ter;
                            }
                        }
                    }
                }
                foreach ( var pair in children ) {
                    pair.Key.Contains = pair.Value.ToArray();
                }
            }

            

        }
    }
}
