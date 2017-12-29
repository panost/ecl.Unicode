using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ecl.Unicode.Cldr {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// http://www.unicode.org/reports/tr35/tr35-dates.html#Date_Field_Symbol_Table
    /// http://cldr.unicode.org/translation/date-time
    /// </remarks>
    public class DataTimePattern {
        public readonly string Text;
        public readonly DateTimeSymbol Symbol;
        public readonly byte Length;

        public bool IsRawText {
            get {
                return Text.HasValue() && Symbol == 0;
            }
        }
        public bool IsArgument {
            get {
                return Symbol == DateTimeSymbol.Argument;
            }
        }
        public DataTimePattern( string rawText ) {
            Text = rawText;
        }

        public DataTimePattern( DateTimeSymbol symbol, byte length ) {
            Symbol = symbol;
            Length = length;
        }
        public DataTimePattern( string name, byte index ) {
            Text = name;
            Length = index;
            Symbol = DateTimeSymbol.Argument;
        }
        public override string ToString() {
            if ( IsRawText ) {
                return Text;
            }
            if ( IsArgument ) {
                
            }
            return Symbol + "[" + Length + "]";
        }
        struct Parser {
            private List<DataTimePattern> _list;
            private int _index;
            private int _length;
            private string _text;
            private StringBuilder _builder;
            public DataTimePattern[] Parse( string pattern, bool withArgs ) {
                _list = new List<DataTimePattern>();
                _builder = new StringBuilder();
                _index = 0;
                _length = pattern.Length;
                _text = pattern;
                for ( ; _index < _length; _index++ ) {

                    var ch = pattern[ _index ];
                    if ( ch == '\'' ) {
                        if ( ParseQuote() ) {
                            continue;
                        }
                    } else if ( ch == '{' ) {
                        if ( withArgs && ParseArg() ) {
                            continue;
                        }
                    } else {
                    SymbolInfo info;
                        if ( _symbolMap.TryGetValue( ch, out info ) ) {
                            AddSymbol( ch, info );
                            continue;
                        }
                    }
                    _builder.Append( ch );
                }
                Flush();
                return _list.ToArray();
            }
            bool ParseArg() {
                var idx = _text.IndexOf( '}', _index + 1 );
                int argLength = idx - _index;
                if ( argLength < 1 ) {
                    return false;
                }
                _index++;
                argLength--;
                string name = _text.Substring( _index, argLength );
                byte argIndex;
                if ( !byte.TryParse( name, NumberStyles.Integer, CultureInfo.InvariantCulture, out argIndex ) ) {
                    argIndex = 0xFF;
                }
                Flush();

                _index = idx;
                _list.Add( new DataTimePattern( name, argIndex ) );

                return true;
            }
            bool ParseQuote() {
                var idx = _text.IndexOf( '\'', _index + 1 );
                int quoteLength = idx - _index;
                if ( quoteLength < 0 ) {
                    return false;
                }
                _index++;
                quoteLength--;
                if ( quoteLength == 0 ) {
                    _builder.Append( '\'' );
                    return true;
                }
                _builder.Append( _text, _index, quoteLength );
                _index = idx;

                while ( _index + 1 < _length && _text[ _index + 1 ] == '\'' ) {
                    _index++;
                    idx = _text.IndexOf( '\'', _index + 1 );
                    if ( idx < 0 ) {
                        _builder.Append( '\'' );
                        break;
                    }
                    quoteLength = idx - _index;

                    _builder.Append( _text, _index, quoteLength );
                    _index = idx;
                }
                
                return true;
            }
            private void AddSymbol( char ch, SymbolInfo info ) {
                Flush();
                int maxLength = info.Times >= 9 ? _length : info.Times;
                int i = _index + 1;
                byte infoLength = 1;
                while ( i < _length ) {
                    if ( _text[ i ] != ch ) {
                        break;
                    }
                    _index++;
                    i++;
                    //maxLength--;
                    infoLength++;
                }
                if ( info.Times < 9 && infoLength > info.Times ) {
                    infoLength = info.Times;
                }
                _list.Add( new DataTimePattern( info.Symbol, infoLength ) );

            }

            private void Flush() {
                if ( _builder.Length > 0 ) {
                    _list.Add( new DataTimePattern( _builder.ToString() ) );
                    _builder.Clear();
                }
            }

            

            
        }

        internal struct SymbolInfo {
            public readonly DateTimeSymbol Symbol;
            /// <summary>
            /// maximum times that this symbol can occur
            /// </summary>
            public readonly byte Times;

            public SymbolInfo( DateTimeSymbol dataTimeSymbol, byte p ) {
                Symbol = dataTimeSymbol;
                Times = p;
            }
            
        }

        public static Tuple<char, DateTimeSymbol, byte>[] GetSymbolTable() {
            var table = new Tuple<char, DateTimeSymbol, byte>[ _symbolMap.Count ];
            int idx = 0;
            foreach ( KeyValuePair<int, SymbolInfo> pair in _symbolMap ) {
                table[idx]=new Tuple<char, DateTimeSymbol, byte>( (char)pair.Key, pair.Value.Symbol, pair.Value.Times );
                idx++;
            }
            return table;
        }
        private static readonly Dictionary<int, SymbolInfo> _symbolMap = GetSymbolMap();

        private static Dictionary<int, SymbolInfo> GetSymbolMap() {
            var map = new Dictionary<int, SymbolInfo>();

            map.Add( 'G', new SymbolInfo( DateTimeSymbol.Era, 5 ) );
            map.Add( 'y', new SymbolInfo( DateTimeSymbol.Year, 9 ) );
            map.Add( 'Y', new SymbolInfo( DateTimeSymbol.YearOfWeeks, 9 ) );
            map.Add( 'u', new SymbolInfo( DateTimeSymbol.ExtendedYear, 9 ) );
            map.Add( 'U', new SymbolInfo( DateTimeSymbol.CyclicYear, 5 ) );
            map.Add( 'r', new SymbolInfo( DateTimeSymbol.RelatedYear, 9 ) );

            map.Add( 'Q', new SymbolInfo( DateTimeSymbol.Quarter, 5 ) );
            map.Add( 'q', new SymbolInfo( DateTimeSymbol.QuarterStandAlone, 5 ) );

            map.Add( 'M', new SymbolInfo( DateTimeSymbol.Month, 5 ) );
            map.Add( 'L', new SymbolInfo( DateTimeSymbol.MonthStandAlone, 5 ) );

            map.Add( 'w', new SymbolInfo( DateTimeSymbol.WeekOfYear, 2 ) );
            map.Add( 'W', new SymbolInfo( DateTimeSymbol.WeekOfMonth, 1 ) );

            map.Add( 'd', new SymbolInfo( DateTimeSymbol.DayOfMonth, 2 ) );
            map.Add( 'D', new SymbolInfo( DateTimeSymbol.DayOfYear, 3 ) );
            map.Add( 'F', new SymbolInfo( DateTimeSymbol.DayOfWeek, 1 ) );
            map.Add( 'g', new SymbolInfo( DateTimeSymbol.JulianDay, 9 ) );

            map.Add( 'E', new SymbolInfo( DateTimeSymbol.WeekDay, 6 ) );
            map.Add( 'e', new SymbolInfo( DateTimeSymbol.WeekDayLocal, 6 ) );
            map.Add( 'c', new SymbolInfo( DateTimeSymbol.WeekDayLocalStandAlone, 6 ) );

            map.Add( 'a', new SymbolInfo( DateTimeSymbol.Period, 5 ) );

            map.Add( 'h', new SymbolInfo( DateTimeSymbol.Hour12, 2 ) );
            map.Add( 'H', new SymbolInfo( DateTimeSymbol.Hour24, 2 ) );

            map.Add( 'K', new SymbolInfo( DateTimeSymbol.SkeletonHour12, 2 ) );
            map.Add( 'k', new SymbolInfo( DateTimeSymbol.SkeletonHour24, 2 ) );

            map.Add( 'm', new SymbolInfo( DateTimeSymbol.Minute, 2 ) );
            map.Add( 's', new SymbolInfo( DateTimeSymbol.Second, 2 ) );
            map.Add( 'S', new SymbolInfo( DateTimeSymbol.SecondFractional, 9 ) );
            map.Add( 'A', new SymbolInfo( DateTimeSymbol.Milliseconds, 9 ) );


            map.Add( ':', new SymbolInfo( DateTimeSymbol.TimeSeparator, 1 ) );

            map.Add( 'z', new SymbolInfo( DateTimeSymbol.Zone, 4 ) );
            map.Add( 'Z', new SymbolInfo( DateTimeSymbol.ZoneISO8601, 5 ) );
            map.Add( 'O', new SymbolInfo( DateTimeSymbol.ZoneGMT, 4 ) );
            map.Add( 'v', new SymbolInfo( DateTimeSymbol.ZoneGeneric, 4 ) );
            map.Add( 'V', new SymbolInfo( DateTimeSymbol.ZoneId, 4 ) );
            map.Add( 'X', new SymbolInfo( DateTimeSymbol.ZoneBasic, 5 ) );
            map.Add( 'x', new SymbolInfo( DateTimeSymbol.ZoneBasicZ, 5 ) );

            return map;
        }
        public static DataTimePattern[] Parse( string s, bool withArgs = false ) {
            Parser p = new Parser();
            return p.Parse( s, withArgs );
        }
    }
}
