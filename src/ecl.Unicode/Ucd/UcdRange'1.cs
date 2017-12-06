namespace eclUnicode.Ucd {
    public class UcdRange<T> : UcdRange {
        public readonly T Owner;

        public UcdRange( int begin, int end, T owner )
            : base( begin, end ) {
            Owner = owner;
        }
    }
}
