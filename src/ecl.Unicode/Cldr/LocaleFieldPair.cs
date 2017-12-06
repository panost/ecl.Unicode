using System;

namespace eclUnicode.Cldr {
    public struct LocaleFieldPair : IEquatable<LocaleFieldPair> {
        private readonly byte _value;
        /// <summary>
        /// 
        /// </summary>
        public LocaleFieldSize Format {
            get {
                return (LocaleFieldSize)( _value & 3 );
            }
        }

        public LocaleFieldType Compose {
            get {
                return (_value & 4) != 0 
                    ? LocaleFieldType.StandAlone
                    : LocaleFieldType.Default;
            }
        }
        public LocaleFieldPair( LocaleFieldSize format, LocaleFieldType compose = LocaleFieldType.Default ) {
            var val = (int)format;
            if ( compose == LocaleFieldType.StandAlone ) {
                val |= 4;
            }
            _value = (byte)val;
        }

        public static implicit operator LocaleFieldPair( LocaleFieldSize value ) {
            return new LocaleFieldPair(value);
        }
        public bool Equals( LocaleFieldPair other ) {
            return other._value == _value;
        }

        public override int GetHashCode() {
            return _value;
        }

        public override string ToString() {
            string s = Format.ToString();
            if ( ( _value & 4 ) != 0 ) {
                s += ", " + Compose.ToString();
            }
            return s;
        }
    }
}
