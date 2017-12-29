using System;
using System.Collections;
using System.Collections.Generic;

namespace ecl.Unicode {
    /// <summary>
    /// Represents zero/null, one, or many elements in an efficient way.
    /// </summary>
    public struct Values<T> : IList<T>, IReadOnlyList<T> {
        private readonly T _value;
        private readonly T[] _values;

        public Values( T value ) {
            _value = value;
            _values = Array.Empty<T>();
        }
        public Values( T[] values ) {
            int len = values == null ? 0 : values.Length;
            if ( len == 0 ) {
                _value = default(T);
                _values = null;
            } else if ( len == 1 ) {
                _value = values[ 0 ];
                _values = Array.Empty<T>();
            } else {
                _value = default(T);
                _values = values;
            }
        }

        public Values<T> Add( Values<T> other ) {
            var count1 = Count;
            var count2 = other.Count;

            if ( count1 == 0 ) {
                return other;
            }

            if ( count2 == 0 ) {
                return this;
            }

            var combined = new T[ count1 + count2 ];
            CopyTo( combined, 0 );
            other.CopyTo( combined, count1 );
            return new Values<T>( combined );
        }

        public Values<T> Add( T value ) {
            var count1 = Count;
            if ( count1 == 0 ) {
                return new Values<T>( value );
            }

            var combined = new T[ count1 + 1 ];
            CopyTo( combined, 0 );
            combined[ count1 ] = value;
            return new Values<T>( combined );
        }

        void ICollection<T>.Add( T item ) {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear() {
            throw new NotSupportedException();
        }

        public T First {
            get {
                if ( _values != null ) {
                    if ( _values.Length == 0 ) {
                        return _value;
                    }
                    return _values[ 0 ];
                }
                return default;
            }
        }
        public bool Contains( T item ) {
            return IndexOf( item ) >= 0;
        }

        public void CopyTo( T[] array, int arrayIndex ) {
            if ( _values != null ) {
                if ( _values.Length == 0 ) {
                    array[ arrayIndex ] = _value;
                } else {
                    Array.Copy( _values, 0, array, arrayIndex, _values.Length );
                }
            }
        }
        public T[] ToArray() {
            return GetArrayValue() ?? Array.Empty<T>();
        }
        private T[] GetArrayValue() {
            if ( _values != null ) {
                if ( _values.Length == 0 )
                    return new[] { _value };
                return _values;
            }
            return null;
        }

        bool ICollection<T>.Remove( T item ) {
            throw new NotSupportedException();
        }

        public int Count {
            get {
                if ( _values == null )
                    return 0;
                int len = _values.Length;
                return len == 0 ? 1 : len;
            }
        }

        bool ICollection<T>.IsReadOnly => true;

        public int IndexOf( T item ) {
            if ( _values != null ) {
                if ( _values.Length == 0 ) {
                    return EqualityComparer<T>.Default.Equals( _value, item ) ? 0 : -1;
                }
                return Array.IndexOf( _values, item );
            }
            return -1;
        }

        void IList<T>.Insert( int index, T item ) {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt( int index ) {
            throw new NotSupportedException();
        }

        public T this[ int index ] {
            get {
                if ( _values != null ) {
                    if ( _values.Length == 0 ) {
                        if ( index == 0 )
                            return _value;
                    } else {
                        return _values[ index ];
                    }
                }
                return Array.Empty<T>()[ index ];
            }
        }
        T IList<T>.this[ int index ] {
            get => this[ index ];
            set => throw new NotSupportedException();
        }

        int IReadOnlyCollection<T>.Count => Count;

        T IReadOnlyList<T>.this[ int index ] => this[ index ];

        public Enumerator GetEnumerator() {
            return new Enumerator( _values, _value );
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T> {
            private readonly T[] _values;
            private T _current;
            private int _index;

            public Enumerator( T[] values, T value ) {
                if ( values == null ) {
                    _index = -1;
                    _values = null;
                } else {
                    _values = values;
                    _index = 0;
                }
                _current = value;
            }

            public bool MoveNext() {
                if ( _index < 0 ) {
                    return false;
                }
                if ( _values.Length == 0 ) {
                    _index = -1;
                    return true;
                }
                if ( _index < _values.Length ) {
                    _current = _values[ _index ];
                    _index++;
                    return true;
                }
                _index = -1;
                return false;
            }

            public T Current => _current;

            object IEnumerator.Current => _current;

            void IEnumerator.Reset() {
                if ( _values != null )
                    _index = 0;
            }

            public void Dispose() {
            }
        }


    }
}
