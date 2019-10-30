using System;
using System.Collections.Generic;

namespace ecl.Unicode.Cldr.Doc {
    internal struct IndexedEntry<T> {
        public int Index;
        public T Value;

        public IndexedEntry( int idx, T p ) {
            Index = idx;
            Value = p;
        }
    }

    public static partial class CldrUtil {
        internal static void SortByIndex<T>( this List<IndexedEntry<T>> list ) {
            list.Sort( ( a, b ) => a.Index.CompareTo( b.Index ) );
        }

        internal static T[] GetValues<T>( this List<IndexedEntry<T>> list ) {
            T[] values = new T[ list.Count ];
            for ( int i = 0; i < values.Length; i++ ) {
                values[ i ] = list[ i ].Value;
            }
            return values;
        }

        public static readonly LocaleFieldSize[] AllEntryFormats = (LocaleFieldSize[])Enum.GetValues( typeof( LocaleFieldSize ) );
        public static readonly LocaleFieldType[] AllEntryComposes = (LocaleFieldType[])Enum.GetValues( typeof( LocaleFieldType ) );

        private static readonly LocaleFieldSize _entryFormatMaxValue = GetEntryFormatMaxValue();

        private static LocaleFieldSize GetEntryFormatMaxValue() {
            return AllEntryFormats[ AllEntryFormats.Length - 1 ];
        }

        public static IEnumerable<LocaleFieldPair> GetClosest( LocaleFieldType compose,
            LocaleFieldSize format ) {
            LocaleFieldSize initFormat = format;

            for( int i = 1; i < (int)_entryFormatMaxValue; i++ ) {
                LocaleFieldSize cformat = (LocaleFieldSize)( (int)initFormat + i );
                if( cformat <= _entryFormatMaxValue ) {
                    yield return new LocaleFieldPair( cformat, compose );
                }
                cformat = (LocaleFieldSize)( (int)initFormat - i );
                if( cformat > 0 ) {
                    yield return new LocaleFieldPair( cformat, compose );
                }
            }
            if( compose == LocaleFieldType.StandAlone ) {
                compose = LocaleFieldType.Default;
                yield return new LocaleFieldPair( initFormat, compose );

                for( int i = 1; i < (int)_entryFormatMaxValue; i++ ) {
                    LocaleFieldSize cformat = (LocaleFieldSize)( (int)initFormat + i );
                    if( cformat <= _entryFormatMaxValue ) {
                        yield return new LocaleFieldPair( cformat, compose );
                    }
                    cformat = (LocaleFieldSize)( (int)initFormat - i );
                    if( cformat > 0 ) {
                        yield return new LocaleFieldPair( cformat, compose );
                    }
                }
            }
        }
        public static Dictionary<LocaleFieldPair, T> GetAll<T>( Func<LocaleFieldSize, LocaleFieldType, T> func, 
            LocaleFieldSize[] formats = null, LocaleFieldType[] composes=null) {
            if ( formats == null ) {
                formats = AllEntryFormats;
            }
            if ( composes == null ) {
                composes = AllEntryComposes;
            }
            var map = new Dictionary<LocaleFieldPair, T>();
            foreach ( LocaleFieldSize format in formats ) {
                foreach ( LocaleFieldType compose in composes ) {
                    var val = func( format, compose );
                    if( val != null ) {
                        map[ new LocaleFieldPair( format, compose ) ] = val;
                    }
                }
            }
            
            return map;
        }

        
    }
}
