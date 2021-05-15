using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ecl.Unicode.Cldr.Doc {
    internal class EnumMap<E> : Dictionary<string, E> where E : struct, IConvertible {
        private readonly string[] _keyValues;

        public string this[ int index ] {
            get {
                if ( (uint)index < _keyValues.Length ) {
                    return _keyValues[ index ];
                }

                return null;
            }
        }

        public EnumMap()
            : base( StringComparer.OrdinalIgnoreCase ) {
            const BindingFlags EnumItemsFlags = BindingFlags.Static | BindingFlags.Public;
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
}
