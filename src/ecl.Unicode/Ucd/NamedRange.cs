namespace ecl.Unicode.Ucd {
    struct NamedRange {
        public readonly int Begin;
        public readonly int End;
        public readonly string Name;

        public NamedRange( int begin, int end, string name ) {
            Begin = begin;
            End = end;
            Name = name;
        }
    }
}
