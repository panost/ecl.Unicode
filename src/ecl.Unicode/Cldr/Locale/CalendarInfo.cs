using System.Collections.Generic;
using ecl.Unicode;
using eclUnicode.Cldr.Doc;

namespace eclUnicode.Cldr.Locale {
    public class CalendarInfo : LdmlAnyNode {
        public string Calendar {
            get {
                return KeyValue;
            }
        }


        internal CalendarInfo() {
        }

        public string[] GetMonths( LocaleFieldSize width, LocaleFieldType compose = LocaleFieldType.Default ) {
            var root = this.Select( new NodePathEntry( "months" ),
                new NodePathEntry( "monthContext", compose.ToCode() ),
                new NodePathEntry( "monthWidth", width.ToCode() )
                );
            if ( root == null ) {
                return null;
            }
            return root.GetList( "month", GetMonthIndex );
        }

        private static int GetMonthIndex( LdmlNode node ) {
            byte idx;
            if ( byte.TryParse( ((LdmlAnyNode)node).KeyValue, out idx ) ) {
                return idx;
            }
            return -1;
        }
        private static int GetDayIndex( LdmlNode node ) {
            return DateField.ParseWeekDay( ( (LdmlAnyNode)node ).KeyValue );
        }
        public string[] GetDays( LocaleFieldSize width, LocaleFieldType compose = LocaleFieldType.Default ) {
            var path = new[] {
                new NodePathEntry( "days" ),
                new NodePathEntry( "dayContext", compose.ToCode() )
            };
            var ctx = this.Select( path );
            LdmlNode found = null;
            if ( ctx != null ) {
                foreach ( int i in XUtil.GetClosest( (int)width, (int)LocaleFieldSize.Wide ) ) {
                    found = ctx.Select( "dayWidth", ( (LocaleFieldSize)i ).ToCode(), LdmlAttribute.Type );
                    if ( found != null ) {
                        break;
                    }
                }
            }
            if ( found == null && compose==LocaleFieldType.StandAlone ) {
                path[ 1 ].Attributes[ 0 ].Value = LocaleFieldType.Default.ToCode();
                ctx = this.Select( path );
                if ( ctx != null ) {
                    foreach ( int i in XUtil.GetClosest( (int)width, (int)LocaleFieldSize.Wide ) ) {
                        found = ctx.Select( "dayWidth", ( (LocaleFieldSize)i ).ToCode(), LdmlAttribute.Type );
                        if ( found != null ) {
                            break;
                        }
                    }
                }
            }
            if ( found == null ) {
                return null;
            }
            
            return found.GetList( "day", GetDayIndex );
        }

        public string[] GetQuarters( LocaleFieldSize width, LocaleFieldType compose = LocaleFieldType.Default ) {
            var root = this.Select( new NodePathEntry( "months" ),
                new NodePathEntry( "monthContext", new LdmlAttributeValue( LdmlAttribute.Type, compose.ToCode() ) ),
                new NodePathEntry( "monthWidth", new LdmlAttributeValue( LdmlAttribute.Type, width.ToCode() ) )
                );
            if ( root == null ) {
                return null;
            }
            return root.GetList( "month", GetMonthIndex );
        }

        public string GetDateFormatPattern( FormatLength length ) {
            var node = this.Select( new NodePathEntry( "dateFormats" ),
                new NodePathEntry( "dateFormatLength", new LdmlAttributeValue( LdmlAttribute.Type, length.ToCode() ) )
                );
            if ( node == null ) {
                return null;
            }
            return node.Select( "dateFormat", "pattern" ).GetText();
        }

        public string GetTimeFormatPattern( FormatLength length ) {
            var node = this.Select( new NodePathEntry( "timeFormats" ),
                new NodePathEntry( "timeFormatLength", new LdmlAttributeValue( LdmlAttribute.Type, length.ToCode() ) )
                );
            if ( node == null ) {
                return null;
            }
            return node.Select( "timeFormat", "pattern" ).GetText();
        }

        public string GetDateTimeFormatPattern( FormatLength length ) {
            var node = this.Select( new NodePathEntry( "dateTimeFormats" ),
                new NodePathEntry( "dateTimeFormatLength", new LdmlAttributeValue( LdmlAttribute.Type, length.ToCode() ) )
                );
            if ( node == null ) {
                return null;
            }
            return node.Select( "dateTimeFormat", "pattern" ).GetText();
        }


        public FormatSizeEntry GetDateTimeFormat() {
            LdmlNode node = this.Select( "dateTimeFormats" );
            FormatSizeEntry entry = new FormatSizeEntry();
            entry.LoadEntries( node, "dateTimeFormatLength", "dateTimeFormat", "pattern" );
            return entry;
        }

        public string GetAvailableFormats( string skeleton ) {
            var node = this.Select( new NodePathEntry( "dateTimeFormats" ),
                new NodePathEntry( "availableFormats" ),
                new NodePathEntry( "dateFormatItem", new LdmlAttributeValue( LdmlAttribute.Id, skeleton ) )
                );
            return node.GetText();
        }

        //public DayPeriodRule[] GetDayPeriods( EntryFormat width, EntryCompose compose = EntryCompose.Default ) {
        //    var node = _root.Select( new NodePathEntry( "timeFormats" ),
        //        new NodePathEntry( "timeFormatLength", new LdmlAttributeValue( LdmlAttribute.Type, width.ToCode() ) )
        //        );
        //    if ( node == null ) {
        //        return null;
        //    }
        //    List<DayPeriodRule> list = new List<DayPeriodRule>();
        //    foreach ( LdmlNode child in node.Children ) {
        //        DayPeriodRuleType type;
        //        if ( child.Name == "dayPeriod" 
        //            && Enum.TryParse( child.KeyValue, true, out type )) {
        //            list.Add( new DayPeriodRule( type, child.Value )  );
        //        }
        //    }
        //    return list.ToArray();
        //}
        public string[] GetDayPeriods( LocaleFieldSize width,
            LocaleFieldType compose = LocaleFieldType.Default ) {
            return new[] {
                GetDayPeriod( "am", width, compose ),
                GetDayPeriod( "pm", width, compose )
            };
        }

        public string GetDayPeriod( string type, LocaleFieldSize width,
            LocaleFieldType compose = LocaleFieldType.Default ) {
            LdmlNode dayPeriods = this.Select( "dayPeriods" );

            var node = dayPeriods.Select( new NodePathEntry( "dayPeriodContext", compose.ToCode() ) );
            NodePathEntry[] path = {
                new NodePathEntry( "dayPeriodWidth", width.ToCode() ),
                new NodePathEntry( "dayPeriod", type )
            };
            foreach ( int w in XUtil.GetClosest( (int)width, (int)LocaleFieldSize.Wide ) ) {
                path[ 0 ].Attributes[ 0 ].Value = ( (LocaleFieldSize)w ).ToCode();

                var rNode = node.Select( path );
                if ( rNode != null ) {
                    return rNode.Value;
                }
            }
            //if ( compose == EntryCompose.StandAlone ) {
            //    return GetDayPeriod( type, width );
            //}
            return null;
        }
    }
}
