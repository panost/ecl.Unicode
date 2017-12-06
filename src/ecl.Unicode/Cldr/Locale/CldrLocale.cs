using System.Collections.Generic;
using ecl.Unicode;
using eclUnicode.Cldr.Doc;

namespace eclUnicode.Cldr.Locale {
    /// <summary>
    /// a lo
    /// </summary>
    public partial class CldrLocale : LdmlDocument {

        #region localeDisplayNames

        public LocaleDisplayNamesNode LocaleDisplayNames {
            get {
                return ResolveRootNode( "localeDisplayNames" ) as LocaleDisplayNamesNode;
            }
        }
        #region localeDisplayPattern

        public string LocalePattern {
            get {
                return SelectNodeText( "localeDisplayNames", "localeDisplayPattern", "localePattern" );
            }
        }
        public string LocaleSeparator {
            get {
                return SelectNodeText( "localeDisplayNames", "localeDisplayPattern", "localeSeparator" );
            }
        }
        public string LocaleKeyTypePattern {
            get {
                return SelectNodeText( "localeDisplayNames", "localeDisplayPattern", "localeKeyTypePattern" );
            }
        }
        #endregion

        

        public string GetLanguageDisplayName( string code ) {
            return SelectNodeKeyText( code, LdmlAttribute.Type, "localeDisplayNames", "languages" );
        }

        public string GetScriptDisplayName( string code ) {
            return SelectNodeKeyText( code, LdmlAttribute.Type, "localeDisplayNames", "scripts" );
        }

        public string GetScriptDisplayName( WritingScript script ) {
            string code = script.ToCode();
            if ( code.HasValue() ) {
                return GetScriptDisplayName( code );
            }
            return null;
        }




        public string GetTerritoryDisplayName( string code ) {
            return SelectNodeKeyText( code, LdmlAttribute.Type, "localeDisplayNames", "territories" );
        }
        public string GetVariantDisplayName( string code ) {
            return SelectNodeKeyText( code, LdmlAttribute.Type, "localeDisplayNames", "variants" );
        }
        public string this[ DisplayKey key ] {
            get {
                string code = key.ToCode();
                if ( code.HasValue() ) {
                    return SelectNodeKeyText( code, LdmlAttribute.Type, "localeDisplayNames", "keys" );
                }
                return null;
            }
        }


        #region measurementSystemNames

        public string MeasurementMetricName {
            get {
                return SelectNodeKeyText( "metric", LdmlAttribute.Type, "localeDisplayNames", "measurementSystemNames" );
            }
        }
        public string MeasurementUkName {
            get {
                return SelectNodeKeyText( "UK", LdmlAttribute.Type, "localeDisplayNames", "measurementSystemNames" );
            }
        }
        public string MeasurementUsName {
            get {
                return SelectNodeKeyText( "US", LdmlAttribute.Type, "localeDisplayNames", "measurementSystemNames" );
            }
        }

        public string this[ MeasurementSystem key ] {
            get {
                switch ( key ) {
                case MeasurementSystem.Metric:
                    return MeasurementMetricName;
                case MeasurementSystem.Us:
                    return MeasurementUsName;
                case MeasurementSystem.Uk:
                    return MeasurementUkName;
                }
                return null;
            }
        }
        #endregion

        #region codePatterns

        public string CodeLanguagePattern {
            get {
                return SelectNodeKeyText( "language", LdmlAttribute.Type, "localeDisplayNames", "codePatterns" );
            }
        }
        public string CodeScriptPattern {
            get {
                return SelectNodeKeyText( "script", LdmlAttribute.Type, "localeDisplayNames", "codePatterns" );
            }
        }
        public string CodeTerritoryPattern {
            get {
                return SelectNodeKeyText( "territory", LdmlAttribute.Type, "localeDisplayNames", "codePatterns" );
            }
        }
        #endregion

        #endregion

        #region DateFields

        public CalendarInfo GetCalendarDates( string calendar = null ) {
            Calendar cal;
            if ( calendar.HasValue() ) {
                cal = Loader.GetCalendar( calendar );
            } else {
                cal = Loader.GetCalendarPreference( TerritoryCode )[ 0 ];
            }
            LdmlNode root = SelectRootNode( "dates", new NodePathEntry( "calendars" ),
                new NodePathEntry( "calendar", 
                new LdmlAttributeValue( LdmlAttribute.Type, cal.Name ) ) );

            return (CalendarInfo)root;
        }

        public NumbersInfo GetNumberInfo() {
            LdmlNode root = ResolveRootNode( "numbers" );

            return (NumbersInfo)root;
        }

        private DateField[] _dateFields;
        /// <summary>
        /// 
        /// </summary>
        public DateField[] DateFields {
            get {
                if ( _dateFields == null ) {
                    _dateFields = LoadDateFields();
                }
                return _dateFields;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public DateField this[ DateFieldType tp ] {
            get {
                foreach ( DateField field in DateFields ) {
                    if ( field.Type == tp ) {
                        return field;
                    }
                }
                return null;
            }
        }

        private DateField[] LoadDateFields() {
            var node = SelectNode( "dates", "fields" );
            if ( node.Document != this ) {
                return ((CldrLocale)node.Document).DateFields;
            }
            List<DateField> list = new List<DateField>();
            foreach ( LdmlAnyNode elm in node.Children ) {
                //JObject obj = elm.Value as JObject;
                    DateFieldType tp = DateField.ParseType( elm.KeyValue );
                    if ( tp != 0 ) {
                        DateField fld = new DateField( tp );
                        fld.Load( elm );
                        list.Add( fld );
                    }
                
            }
            return list.ToArray();
        }
        #endregion

        public CharactersNode Characters {
            get {
                return ResolveRootNode( "characters" ) as CharactersNode;
            }
        }
        private FormatSizeEntry _unitPatternPer;
        /// <summary>
        /// 
        /// </summary>
        public FormatSizeEntry UnitPatternPer {
            get {
                if ( _units == null ) {
                    _units = LoadUnits();
                }
                return _unitPatternPer;
            }
        }
        

        private Dictionary<CldrUnit, UnitOfMeasure> _units;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<CldrUnit, UnitOfMeasure> Units {
            get {
                if ( _units == null ) {
                    _units = LoadUnits();
                }
                return _units;
            }
        }

        private static void LoadUnit( FormatLength size, LdmlNode root,
            Dictionary<CldrUnit, UnitOfMeasure> map, FormatSizeEntry compoundUnitPattern ) {
                foreach ( LdmlNode node in root.Children ) {
                switch ( node.Name ) {
                case "compoundUnit":
                    foreach ( LdmlNode child in node.Children ) {
                        if ( child.Name == "compoundUnitPattern" ) {
                            compoundUnitPattern[size] = child.Value;
                            break;
                        }
                    }
                    break;
                case "unit":
                    LdmlTypeUnitNode unitNode = node as LdmlTypeUnitNode;
                    if ( unitNode != null ) {
                        UnitOfMeasure measure;
                        if ( !map.TryGetValue( unitNode.Unit, out measure ) ) {
                            measure = new UnitOfMeasure( unitNode.Unit );
                            map.Add( unitNode.Unit, measure );
                        }
                        measure.Load( size, unitNode );
                    }
                    break;
                }
            }
        }
        private Dictionary<CldrUnit, UnitOfMeasure> LoadUnits() {
            LdmlNode root = ResolveRootNode( "units" );
            if ( root.Document != this ) {
                _unitPatternPer = ((CldrLocale)root.Document).UnitPatternPer;
                return ((CldrLocale)root.Document).Units;
            }
            _unitPatternPer = new FormatSizeEntry();
            
            var map = new Dictionary<CldrUnit, UnitOfMeasure>();
            foreach ( LdmlNode child in root.Children ) {
                LdmlTypeLengthNode lnode = child as LdmlTypeLengthNode;
                if ( lnode != null && lnode.Name == "unitLength" ) {
                    LoadUnit( lnode.Length, lnode, map, _unitPatternPer );
                }
            }
            return map;
        }

        public string Concat( string a, string b ) {
            if ( a.HasValue() ) {
                if ( b.HasValue() ) {
                    string format = LocaleSeparator;
                    if ( format.HasValue() ) {
                        return string.Format( format, a, b );
                    }
                    return a + ", " + b;
                }
                return a;
            }
            return b;
        }
        public string GetFullName( CldrLocale locale ) {
            var dispNames = locale.LocaleDisplayNames;
            string text;
            if ( dispNames.LanguageNames.TryGetValue( Name, out text ) ) {
                return text;
            }
            if ( _language.HasValue() && dispNames.LanguageNames.TryGetValue( _language, out text ) ) {
                string suffix = "";
                if ( _territory != null ) {
                    suffix = dispNames.TerritoryNames.GetValueOrDefault( _territory );
                }
                if ( _script != 0 ) {
                    suffix = locale.Concat( suffix, locale.GetScriptDisplayName( _script ) );
                }
                if ( _variant.HasValue() ) {
                    suffix = locale.Concat( suffix, locale.GetVariantDisplayName( _variant ) );
                }
                if ( suffix.HasValue() ) {
                    string format = locale.LocalePattern;
                    if ( format.HasValue() ) {
                        text = string.Format( format, text, suffix );
                    } else {
                        text += " (" + suffix + ")";
                    }
                }
                return text;
            }
            return null;
        }
        internal void ResolveDescription( CldrLocale provider ) {
            _description = GetFullName( provider ) ?? Name;
        }

    }
}
