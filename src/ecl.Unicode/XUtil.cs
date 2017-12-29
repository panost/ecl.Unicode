using System;
using System.Collections.Generic;
using System.Reflection;
using ecl.Unicode.Cldr;
using ecl.Unicode.Cldr.Doc;
using JetBrains.Annotations;

namespace ecl.Unicode {
    public static partial class XUtil {
        public static Dictionary<string, WritingScript> GetScriptMap() {
            var map = new Dictionary<string, WritingScript>( StringComparer.OrdinalIgnoreCase );
            foreach ( var pair in GetScriptValues() ) {
                map.Add( pair.Key, pair.Value );
            }
            return map;
        }
        public static string ToCode(this WritingScript script) {
            string code;
            if ( !_scriptCodes.TryGetValue( (short)script, out code ) ) {
                return null;
            }
            return code;
        }

        internal static int BinaryFindItem<T, M>( this T[] entries, M value ) 
            where M : IComparable<T> {
            int length = entries.Length;
            int index = 0;
            while( length > 0 ) {
                int mid = length / 2;
                int c = value.CompareTo( entries[ index + mid ] );
                if( c == 0 )
                    return index + mid;
                if( c < 0 ) {
                    index += mid + 1;
                    length -= mid + 1;
                } else {
                    length = mid;
                }
            }
            return ~index;
        }

        struct BinaryFinder<T, M> where T : IComparable<M> {
            public readonly T[] Entries;
            public readonly M Value;

            public BinaryFinder( T[] entries, M value ) {
                Entries = entries;
                Value = value;
            }
            public int Search( int index, int length ) {
                while( length > 0 ) {
                    int mid = length / 2;
                    int c = Entries[ index + mid ].CompareTo( Value );
                    if( c == 0 )
                        return index + mid;
                    if( c < 0 ) {
                        index += mid + 1;
                        length -= mid + 1;
                    } else {
                        length = mid;
                    }
                }
                return ~index;
            }
            
            public int GetRange( int index, int length, out int end ) {
                const int LinearLimit = 6;

                while( length > 0 ) {
                    int midLength = length / 2;
                    int mid = index + midLength;
                    int c = Entries[ mid ].CompareTo( Value );
                    if( c == 0 ) {
                        //return mid;
                        if( midLength > LinearLimit ) {
                            end = BinaryMax( mid, index + length - 1 );
                            return BinaryMin( mid, index );
                        }
                        length += index;
                        c = mid - 1;
                        while( c >= index && Entries[c].CompareTo( Value ) == 0 ) {
                            c--;
                        }
                        index = c + 1; // min
                        c = mid + 1;
                        while( c < length && Entries[ c ].CompareTo( Value ) == 0 ) {
                            c++;
                        }
                        end = c - 1;
                        return index;
                    }
                    if( c < 0 ) {
                        index = mid + 1;
                        length -= midLength + 1;
                    } else {
                        length = midLength;
                    }
                }
                end = 0;
                return ~index;
            }

            private int BinaryMax( int found, int last ) {
                int max = found + 1;
                while ( max <= last ) {
                    int newIndex = Search( max, last - max + 1 );
                    if ( newIndex < 0 ) {
                        break;
                    }
                    max = newIndex + 1;
                }
                return max - 1;
            }
            private int BinaryMin( int found, int index ) {
                int min = found - 1;
                while ( min >= index ) {
                    int newIndex = Search( index, min - index + 1 );
                    if ( newIndex < 0 ) {
                        break;
                    }
                    min = newIndex - 1;
                }
                return min + 1;
            }
        }
        internal static int BinaryFind<T, M>( this T[] entries, M value ) 
            where T : IComparable<M> {
            return new BinaryFinder<T,M>( entries, value ).Search( 0, entries.Length );
        }

        internal static int GetRange<T, M>( this T[] entries, M value, out int end )
    where T : IComparable<M> {
            return new BinaryFinder<T, M>( entries, value ).GetRange( 0, entries.Length, out end );
        }

        

        internal static int IndexOf( this List<int> list, List<int> values ) {
            int searchLen = values.Count;
            for( int i = 0; i <= list.Count - searchLen; i++ ) {
                bool found = true;
                for( int j = 0; j < searchLen; j++ ) {
                    if( list[ i + j ] != values[ j ] ) {
                        found = false;
                        break;
                    }
                }
                if( found ) {
                    return i;
                }
            }
            return -1;
        }

        internal static void Increment<T>( this Dictionary<T, int> m, T value ) {
            int count;
            if( !m.TryGetValue( value, out count ) ) {
                count = 0;
            }
            m[ value ] = count + 1;
        }

        public static IEnumerable<int> GetClosest( int value, int maxValue, int minValue = 0 ) {
            yield return value;
            int width = maxValue - minValue;
            for ( int i = 1; i < width; i++ ) {
                int val = value + i;
                if ( value <= maxValue ) {
                    yield return val;
                }
                val = value - i;
                if ( value >= minValue ) {
                    yield return val;
                }
            }
        }

        internal static string GetValue( this List<AttributeValue> list, string name ) {
            foreach ( var a in list ) {
                if ( a.Name == name )
                    return a.Value;
            }
            return null;
        }

        internal static T GetOrCreate<T>( this Dictionary<string, T> map, string name )
            where T : CodeObjectBase, new() {
            T meta;
            if ( !map.TryGetValue( name, out meta ) ) {
                meta = new T();
                meta._code = name;
                //if( name.SameName( "aa" ) ) {
                //    Debug.Write( "a" );
                //}
                map.Add( name, meta );
            }
            return meta;
        }



        private static readonly char[] _spaces = { ' ' };

        public static string[] SplitAtSpaces( this string a ) {
            return a.Split( _spaces, StringSplitOptions.RemoveEmptyEntries );
        }
        public static T[] SplitAtSpaces<T>( this string a, Dictionary<string,T> map ) 
            where T : CodeObjectBase, new() {
            var codes = a.Split( _spaces, StringSplitOptions.RemoveEmptyEntries );
            var values = new T[ codes.Length ];
            for ( int i = 0; i < codes.Length; i++ ) {
                values[ i ] = map.GetOrCreate( codes[ i ] );
            }
            return values;
        }
        [ContractAnnotation( "null=>false" )]
        internal static bool HasValue( this string a ) {
            return !String.IsNullOrEmpty( a );
        }
        internal static bool SameName( this string a, string b ) {
            return String.Equals( a, b, StringComparison.OrdinalIgnoreCase );
        }
        public static List<K> GetList<T, K>( this Dictionary<T, List<K>> map, T key ) {
            List<K> list;

            if ( !map.TryGetValue( key, out list ) ) {
                list = new List<K>();
                map.Add( key, list );
            }
            return list;
        }
        public static void AddGroup<T, K>( this Dictionary<T, List<K>> map, T key, K value ) {
            List<K> list;

            if ( !map.TryGetValue( key, out list ) ) {
                list = new List<K>();
                map.Add( key, list );
            }
            list.Add( value );
        }

        public static K GetValueOrDefault<T, K>( this Dictionary<T, K> map, T key ) {
            K value;
            map.TryGetValue( key, out value );
            return value;
        }

        internal static EnumKeyEntry[] ToTable( this Dictionary<int, string> map ) {
            List<EnumKeyEntry> list = new List<EnumKeyEntry>( map.Count );
            foreach ( KeyValuePair<int, string> elm in map ) {
                list.Add( new EnumKeyEntry(elm.Key,elm.Value) );
            }
            list.Sort( CompareKeyEntry );
            return list.ToArray();
        }

        internal static int CompareKeyEntry( EnumKeyEntry a, EnumKeyEntry b ) {
            return a.Type - b.Type;
        }

        internal static string GetText( this EnumKeyEntry[] table, int key ) {
            if ( table != null ) {
                for ( int i = 0; i < table.Length; i++ ) {
                    if ( key == table[ i ].Type ) {
                        return table[ i ].Text;
                    }
                }
            }
            return null;
        }

        private class EnumMap<E> : Dictionary<string, E> where E : struct, IConvertible {
            private readonly string[] _keyValues;

            public string this[ int index ] {
                get {
                    if ( (uint)index < _keyValues.Length ) {
                        return _keyValues[ index ];
                    }
                    return null;
                }
            }
            public EnumMap() {
                const BindingFlags EnumItemsFlags = BindingFlags.Static | BindingFlags.Public ;
                var enumType = typeof( E );
                var info = enumType.GetTypeInfo();
                FieldInfo[] fields = info.GetFields( EnumItemsFlags );
                List<string> list = new List<string>( fields.Length + 1 );

                foreach ( FieldInfo field in fields ) {
                    var conv = (IConvertible)field.GetValue( null );
                    int idx = conv.ToInt32( null );

                    var keycode = field.GetCustomAttribute<KeyCodeAttribute>();
                    if ( keycode != null ) {
                        this[ keycode.Code ] = (E)conv;
                        while ( list.Count <= idx ) {
                            list.Add( null );
                        }
                        list[ idx ] = keycode.Code;
                    }
                }
                _keyValues = list.ToArray();

            }
        }
        #region DisplayKey
        private static readonly EnumMap<DisplayKey> _displayKeyMap = new EnumMap<DisplayKey>();

        public static string ToCode( this DisplayKey key ) {
            return _displayKeyMap[ (int)key ];
        }
        public static DisplayKey ParseDisplayKey( string text ) {
            if ( text.HasValue() ) {
                DisplayKey key;
                if ( _displayKeyMap.TryGetValue( text, out key ) ) {
                    return key;
                }
            }
            return 0;
        }

        #endregion

        #region DisplayNameType

        private static readonly Dictionary<string, DisplayNameType> _displayNameTypeMap = GetDisplayNameTypeMap();

        private static Dictionary<string, DisplayNameType> GetDisplayNameTypeMap() {
            var map = new Dictionary<string, DisplayNameType>( StringComparer.OrdinalIgnoreCase );

            map.Add( "calendar", DisplayNameType.Calendar );
            map.Add( "collation", DisplayNameType.Collation );
            map.Add( "numbers", DisplayNameType.Numbers );
            map.Add( "colAlternate", DisplayNameType.ColAlternate );
            map.Add( "colBackwards", DisplayNameType.ColBackwards );
            map.Add( "colCaseFirst", DisplayNameType.colCaseFirst );
            map.Add( "colCaseLevel", DisplayNameType.colCaseLevel );
            map.Add( "colHiraganaQuaternary", DisplayNameType.colHiraganaQuaternary );
            map.Add( "colNormalization", DisplayNameType.colNormalization );
            map.Add( "colNumeric", DisplayNameType.colNumeric );
            map.Add( "colStrength", DisplayNameType.colStrength );
            map.Add( "va", DisplayNameType.va );


            return map;
        }

        public static DisplayNameType ParseDisplayNameType( string text ) {
            if ( text.HasValue() ) {
                DisplayNameType key;
                if ( _displayNameTypeMap.TryGetValue( text, out key ) ) {
                    return key;
                }
            }
            return 0;
        }

        #endregion

        #region WellKnownUnit
        

        //private static readonly Dictionary<string, WellKnownUnit> _unitNames = GetUnitNames();

        //private static Dictionary<string, WellKnownUnit> GetUnitNames() {
        //    var map = new Dictionary<string, WellKnownUnit>( StringComparer.OrdinalIgnoreCase );
        //    map.Add( "acceleration-g-force", WellKnownUnit.AccelerationGForce );
        //    map.Add( "acceleration-meter-per-second-squared", WellKnownUnit.AccelerationMeterPerSecondSquared );
        //    map.Add( "angle-arc-minute", WellKnownUnit.AngleArcMinute );
        //    map.Add( "angle-arc-second", WellKnownUnit.AngleArcSecond );
        //    map.Add( "angle-degree", WellKnownUnit.AngleDegree );
        //    map.Add( "angle-radian", WellKnownUnit.AngleRadian );
        //    map.Add( "angle-revolution", WellKnownUnit.AngleRevolution );
        //    map.Add( "area-acre", WellKnownUnit.AreaAcre );
        //    map.Add( "area-hectare", WellKnownUnit.AreaHectare );
        //    map.Add( "area-square-centimeter", WellKnownUnit.AreaSquareCentimeter );
        //    map.Add( "area-square-foot", WellKnownUnit.AreaSquareFoot );
        //    map.Add( "area-square-inch", WellKnownUnit.AreaSquareInch );
        //    map.Add( "area-square-kilometer", WellKnownUnit.AreaSquareKilometer );
        //    map.Add( "area-square-meter", WellKnownUnit.AreaSquareMeter );
        //    map.Add( "area-square-mile", WellKnownUnit.AreaSquareMile );
        //    map.Add( "area-square-yard", WellKnownUnit.AreaSquareYard );
        //    map.Add( "consumption-liter-per-100kilometers", WellKnownUnit.ConsumptionLiterPer100Kilometers );
        //    map.Add( "consumption-liter-per-kilometer", WellKnownUnit.ConsumptionLiterPerKilometer );
        //    map.Add( "consumption-mile-per-gallon", WellKnownUnit.ConsumptionMilePerGallon );
        //    map.Add( "digital-bit", WellKnownUnit.DigitalBit );
        //    map.Add( "digital-byte", WellKnownUnit.DigitalByte );
        //    map.Add( "digital-gigabit", WellKnownUnit.DigitalGigabit );
        //    map.Add( "digital-gigabyte", WellKnownUnit.DigitalGigabyte );
        //    map.Add( "digital-kilobit", WellKnownUnit.DigitalKilobit );
        //    map.Add( "digital-kilobyte", WellKnownUnit.DigitalKilobyte );
        //    map.Add( "digital-megabit", WellKnownUnit.DigitalMegabit );
        //    map.Add( "digital-megabyte", WellKnownUnit.DigitalMegabyte );
        //    map.Add( "digital-terabit", WellKnownUnit.DigitalTerabit );
        //    map.Add( "digital-terabyte", WellKnownUnit.DigitalTerabyte );
        //    map.Add( "duration-day", WellKnownUnit.DurationDay );
        //    map.Add( "duration-hour", WellKnownUnit.DurationHour );
        //    map.Add( "duration-microsecond", WellKnownUnit.DurationMicrosecond );
        //    map.Add( "duration-millisecond", WellKnownUnit.DurationMillisecond );
        //    map.Add( "duration-minute", WellKnownUnit.DurationMinute );
        //    map.Add( "duration-month", WellKnownUnit.DurationMonth );
        //    map.Add( "duration-nanosecond", WellKnownUnit.DurationNanosecond );
        //    map.Add( "duration-second", WellKnownUnit.DurationSecond );
        //    map.Add( "duration-week", WellKnownUnit.DurationWeek );
        //    map.Add( "duration-year", WellKnownUnit.DurationYear );
        //    map.Add( "duration-century", WellKnownUnit.DurationCentury );

        //    map.Add( "duration-day-person", WellKnownUnit.DurationDayPerson );
        //    map.Add( "duration-month-person", WellKnownUnit.DurationMonthPerson );
        //    map.Add( "duration-week-person", WellKnownUnit.DurationWeekPerson );
        //    map.Add( "duration-year-person", WellKnownUnit.DurationYearPerson );

        //    map.Add( "electric-ampere", WellKnownUnit.ElectricAmpere );
        //    map.Add( "electric-milliampere", WellKnownUnit.ElectricMilliampere );
        //    map.Add( "electric-ohm", WellKnownUnit.ElectricOhm );
        //    map.Add( "electric-volt", WellKnownUnit.ElectricVolt );
        //    map.Add( "energy-calorie", WellKnownUnit.EnergyCalorie );
        //    map.Add( "energy-foodcalorie", WellKnownUnit.EnergyFoodcalorie );
        //    map.Add( "energy-joule", WellKnownUnit.EnergyJoule );
        //    map.Add( "energy-kilocalorie", WellKnownUnit.EnergyKilocalorie );
        //    map.Add( "energy-kilojoule", WellKnownUnit.EnergyKilojoule );
        //    map.Add( "energy-kilowatt-hour", WellKnownUnit.EnergyKilowattHour );
        //    map.Add( "frequency-gigahertz", WellKnownUnit.FrequencyGigahert );
        //    map.Add( "frequency-hertz", WellKnownUnit.FrequencyHert );
        //    map.Add( "frequency-kilohertz", WellKnownUnit.FrequencyKilohert );
        //    map.Add( "frequency-megahertz", WellKnownUnit.FrequencyMegahert );
        //    map.Add( "length-astronomical-unit", WellKnownUnit.LengthAstronomicalUnit );
        //    map.Add( "length-centimeter", WellKnownUnit.LengthCentimeter );
        //    map.Add( "length-decimeter", WellKnownUnit.LengthDecimeter );
        //    map.Add( "length-fathom", WellKnownUnit.LengthFathom );
        //    map.Add( "length-foot", WellKnownUnit.LengthFoot );
        //    map.Add( "length-furlong", WellKnownUnit.LengthFurlong );
        //    map.Add( "length-inch", WellKnownUnit.LengthInch );
        //    map.Add( "length-kilometer", WellKnownUnit.LengthKilometer );
        //    map.Add( "length-light-year", WellKnownUnit.LengthLightYear );
        //    map.Add( "length-meter", WellKnownUnit.LengthMeter );
        //    map.Add( "length-micrometer", WellKnownUnit.LengthMicrometer );
        //    map.Add( "length-mile", WellKnownUnit.LengthMile );
        //    map.Add( "length-mile-scandinavian", WellKnownUnit.LengthMileScandinavian );
        //    map.Add( "length-millimeter", WellKnownUnit.LengthMillimeter );
        //    map.Add( "length-nanometer", WellKnownUnit.LengthNanometer );
        //    map.Add( "length-nautical-mile", WellKnownUnit.LengthNauticalMile );
        //    map.Add( "length-parsec", WellKnownUnit.LengthParsec );
        //    map.Add( "length-picometer", WellKnownUnit.LengthPicometer );
        //    map.Add( "length-yard", WellKnownUnit.LengthYard );
        //    map.Add( "light-lux", WellKnownUnit.LightLux );
        //    map.Add( "mass-carat", WellKnownUnit.MassCarat );
        //    map.Add( "mass-gram", WellKnownUnit.MassGram );
        //    map.Add( "mass-kilogram", WellKnownUnit.MassKilogram );
        //    map.Add( "mass-metric-ton", WellKnownUnit.MassMetricTon );
        //    map.Add( "mass-microgram", WellKnownUnit.MassMicrogram );
        //    map.Add( "mass-milligram", WellKnownUnit.MassMilligram );
        //    map.Add( "mass-ounce", WellKnownUnit.MassOunce );
        //    map.Add( "mass-ounce-troy", WellKnownUnit.MassOunceTroy );
        //    map.Add( "mass-pound", WellKnownUnit.MassPound );
        //    map.Add( "mass-stone", WellKnownUnit.MassStone );
        //    map.Add( "mass-ton", WellKnownUnit.MassTon );
        //    map.Add( "power-gigawatt", WellKnownUnit.PowerGigawatt );
        //    map.Add( "power-horsepower", WellKnownUnit.PowerHorsepower );
        //    map.Add( "power-kilowatt", WellKnownUnit.PowerKilowatt );
        //    map.Add( "power-megawatt", WellKnownUnit.PowerMegawatt );
        //    map.Add( "power-milliwatt", WellKnownUnit.PowerMilliwatt );
        //    map.Add( "power-watt", WellKnownUnit.PowerWatt );
        //    map.Add( "pressure-hectopascal", WellKnownUnit.PressureHectopascal );
        //    map.Add( "pressure-inch-hg", WellKnownUnit.PressureInchHg );
        //    map.Add( "pressure-millibar", WellKnownUnit.PressureMillibar );
        //    map.Add( "pressure-millimeter-of-mercury", WellKnownUnit.PressureMillimeterOfMercury );
        //    map.Add( "pressure-pound-per-square-inch", WellKnownUnit.PressurePoundPerSquareInch );
        //    map.Add( "proportion-karat", WellKnownUnit.ProportionKarat );
        //    map.Add( "speed-kilometer-per-hour", WellKnownUnit.SpeedKilometerPerHour );
        //    map.Add( "speed-meter-per-second", WellKnownUnit.SpeedMeterPerSecond );
        //    map.Add( "speed-mile-per-hour", WellKnownUnit.SpeedMilePerHour );
        //    map.Add( "speed-knot", WellKnownUnit.SpeedKnot );
        //    map.Add( "temperature-generic", WellKnownUnit.TemperatureGeneric );
        //    map.Add( "temperature-celsius", WellKnownUnit.TemperatureCelsius );
        //    map.Add( "temperature-fahrenheit", WellKnownUnit.TemperatureFahrenheit );
        //    map.Add( "temperature-kelvin", WellKnownUnit.TemperatureKelvin );
        //    map.Add( "volume-acre-foot", WellKnownUnit.VolumeAcreFoot );
        //    map.Add( "volume-bushel", WellKnownUnit.VolumeBushel );
        //    map.Add( "volume-centiliter", WellKnownUnit.VolumeCentiliter );
        //    map.Add( "volume-cubic-centimeter", WellKnownUnit.VolumeCubicCentimeter );
        //    map.Add( "volume-cubic-foot", WellKnownUnit.VolumeCubicFoot );
        //    map.Add( "volume-cubic-inch", WellKnownUnit.VolumeCubicInch );
        //    map.Add( "volume-cubic-kilometer", WellKnownUnit.VolumeCubicKilometer );
        //    map.Add( "volume-cubic-meter", WellKnownUnit.VolumeCubicMeter );
        //    map.Add( "volume-cubic-mile", WellKnownUnit.VolumeCubicMile );
        //    map.Add( "volume-cubic-yard", WellKnownUnit.VolumeCubicYard );
        //    map.Add( "volume-cup", WellKnownUnit.VolumeCup );
        //    map.Add( "volume-cup-metric", WellKnownUnit.VolumeCupMetric );
        //    map.Add( "volume-deciliter", WellKnownUnit.VolumeDeciliter );
        //    map.Add( "volume-fluid-ounce", WellKnownUnit.VolumeFluidOunce );
        //    map.Add( "volume-gallon", WellKnownUnit.VolumeGallon );
        //    map.Add( "volume-hectoliter", WellKnownUnit.VolumeHectoliter );
        //    map.Add( "volume-liter", WellKnownUnit.VolumeLiter );
        //    map.Add( "volume-megaliter", WellKnownUnit.VolumeMegaliter );
        //    map.Add( "volume-milliliter", WellKnownUnit.VolumeMilliliter );
        //    map.Add( "volume-pint", WellKnownUnit.VolumePint );
        //    map.Add( "volume-pint-metric", WellKnownUnit.VolumePintMetric );
        //    map.Add( "volume-quart", WellKnownUnit.VolumeQuart );
        //    map.Add( "volume-tablespoon", WellKnownUnit.VolumeTablespoon );
        //    map.Add( "volume-teaspoon", WellKnownUnit.VolumeTeaspoon );
        //    return map;
        //}
        //private static readonly Dictionary<int, string> _unitValues = GetUnitValues();

        //private static Dictionary<int, string> GetUnitValues() {
        //    Dictionary<int, string> map = new Dictionary<int, string>();
        //    foreach ( KeyValuePair<string, WellKnownUnit> pair in _unitNames ) {
        //        map[ (int)pair.Value ] = pair.Key;
        //    }
        //    return map;
        //}
        //public static bool TryParseUnit( string name, out WellKnownUnit unit ) {
        //    return _unitNames.TryGetValue( name, out unit );
        //}
        //public static string ToCode( this WellKnownUnit unit ) {
        //    string name;
        //    if ( _unitValues.TryGetValue( (int)unit, out name ) ) {
        //        return name;
        //    }
        //    return "";
        //}
        #endregion
    }
}
