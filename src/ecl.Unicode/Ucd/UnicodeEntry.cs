using System;
using System.Diagnostics;
using System.Text;

namespace ecl.Unicode.Ucd {
    [DebuggerDisplay("{Name},{CodeValue.ToString(\"X6\")} '{ToString()}'")]
    public struct UnicodeEntry : IComparable<int> {
        public string Name;
        public string EnumName;
        public string Comment;
        public string OldName;
        public int CodeValue;
        internal NumericValue _value;
        public int Uppercase;
        public int LowerCase;
        public int TitleCase;

        public int ToUpper() {
            if ( Uppercase != 0 )
                return Uppercase;
            return CodeValue;
        }
        /// <summary>
        /// if negative then it's the count
        /// and _decomposingStart2 an index in the decomposing map
        /// else it's the first decomposing char
        /// and _decomposingStart2 is the second char
        /// </summary>
        internal int _decomposingStart;
        internal int _decomposingStart2;
        public bool Mirrored;
        //public byte UppercaseLength;
        //public byte LowerCaseLength;
        //public byte TitleCaseLength;
        public DecomposingTag Decomposing;
        public UnicodeCharacterType Category;
        public CombingClass Combing;
        public BidirectionalCategory Bidirectional;
        internal NumericEntryType _numericType;
        public WritingScript Script;
        public int ScriptIndex;
        public byte ScriptCount;

        public int DecomposingLength {
            get {
                int cnt = _decomposingStart;
                if ( cnt <= 0 )
                    return -cnt;
                if ( cnt > 0 ) {
                    cnt = 1;
                    if ( _decomposingStart2 != 0 ) {
                        cnt++;
                    }
                }
                return cnt;
            }
        }
        public bool IsDecimalDigit {
            get {
                return _numericType == NumericEntryType.Decimal;
            }
        }
        public sbyte DecimalValue {
            get {
                if ( IsDecimalDigit ) {
                    return (sbyte)_value.Value;
                }
                return -1;
            }
        }
        public bool IsDigit {
            get {
                return IsDecimalDigit
                    || _numericType == NumericEntryType.Digit;
            }
        }
        public sbyte DigitValue {
            get {
                if ( IsDigit ) {
                    return (sbyte)_value.Value;
                }
                return -1;
            }
        }
        public bool IsFraction {
            get {
                return _numericType == NumericEntryType.Fraction;
            }
        }
        public bool GetFraction(out short numerator, out short denominator) {
            if ( IsFraction ) {
                numerator = _value.Numerator;
                denominator = _value.Denominator;
                return true;
            }
            numerator = denominator = 0;
            return false;
        }

        public bool HasIntegerValue {
            get {
                switch ( _numericType ) {
                case NumericEntryType.None:
                case NumericEntryType.Fraction:
                    return false;
                }
                return true;
            }
        }

        public long IntegerValue {
            get {
                switch ( _numericType ) {
                case NumericEntryType.None:
                case NumericEntryType.Fraction:
                    return long.MaxValue;
                //case NumericEntryType.Value:
                //    return _value.Value;
                case NumericEntryType.Billions:
                    return _value.Value * (long)1000000000;
                }
                return _value.Value;
            }
        }

        public bool IsNumericValue {
            get {
                return _numericType != NumericEntryType.None;
            }
        }

        public double NumericValue {
            get {
                switch( _numericType ) {
                case NumericEntryType.None:
                    return double.NaN;
                case NumericEntryType.Fraction:
                    return (double)_value.Numerator / (double)_value.Denominator;
                case NumericEntryType.Billions:
                    return _value.Value * (double)1000000000;
                }
                return _value.Value;
            }
        }


        public int CompareTo( int other ) {
            return CodeValue.CompareTo( other );
        }

        public override string ToString() {
            return char.ConvertFromUtf32( CodeValue );
            //if ( CodeValue > char.MaxValue ) {
            //    char[] surrogate = new char[ 2 ];
            //    int utf32 = CodeValue - 0x10000;
            //    const char HIGH_SURROGATE_START = '\ud800';
            //    const char HIGH_SURROGATE_END = '\udbff';
            //    const char LOW_SURROGATE_START = '\udc00';
            //    const char LOW_SURROGATE_END = '\udfff';

            //    surrogate[ 0 ] = (char)( ( utf32 / 0x400 ) + (int)HIGH_SURROGATE_START );
            //    surrogate[ 1 ] = (char)( ( utf32 % 0x400 ) + (int)LOW_SURROGATE_START );
            //    return new string( surrogate );
            //}
            //return ( (char)CodeValue ).ToString();
        }

        public void AppendCharTo( StringBuilder b ) {
            if ( CodeValue > char.MaxValue ) {
                char[] surrogate = new char[ 2 ];
                int utf32 = CodeValue - 0x10000;
                const char HIGH_SURROGATE_START = '\ud800';
                const char HIGH_SURROGATE_END = '\udbff';
                const char LOW_SURROGATE_START = '\udc00';
                const char LOW_SURROGATE_END = '\udfff';

                b.Append( (char)( ( utf32 / 0x400 ) + (int)HIGH_SURROGATE_START ) );
                b.Append( (char)( ( utf32 % 0x400 ) + (int)LOW_SURROGATE_START ) );
            } else {
                b.Append( (char)CodeValue );
            }

        }
    }
}
