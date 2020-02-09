using System;
using System.Collections.Generic;
using System.Text;

namespace ecl.Unicode.Ucd {
    public struct EnumRange<T> {
        public readonly int Begin;
        public readonly int End;
        public readonly T Value;

        public EnumRange( int begin, int end, T name ) {
            Begin = begin;
            End = end;
            Value = name;
        }
    }
}
