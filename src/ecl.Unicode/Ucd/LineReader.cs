using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace eclUnicode.Ucd {
    [Flags]
    public enum TextReaderOptions {
        /// <summary>
        /// if set, empty lines will have an empty segment
        /// otherwise they will not have any segment
        /// </summary>
        KeepEmptyLines = 1,

        /// <summary>
        /// if set it will not trim whitespace from the line start
        /// </summary>
        DontTrimLines = 2,

        /// <summary>
        /// if set it will not search for comment before processing segments
        /// </summary>
        DontCommentLine = 4,

        /// <summary>
        /// if set it will search for comment on the last segment
        /// </summary>
        SeachCommentAtEnd = 8 | 4
    };
    public class LineReader : IDisposable {
        private Stream _stream;
        private byte[] _buffer;
        private int _length;
        private int _index;
        private long _offset;
        private const int LineSize = 512;
        private TextReaderOptions _options;

        public LineReader( Stream stream, TextReaderOptions options ) {
            _stream = stream;
            _buffer = new byte[ LineSize ];
            _options = options;
        }

        private int IndexOf( char ch, int from, int length ) {
            return Array.IndexOf( _buffer, (byte)ch, from, length );
        }
        private int IndexOf( int from ) {
            return Array.IndexOf( _buffer, (byte)'\n', from, _length - from );
        }

        private static bool IsWhiteSpace( byte a ) {
            return a <= '\r' || a == ' ';
        }
        private int TrimStart( ref int index, int length ) {
            int i = index;
            for ( ; length > 0; i++ ) {
                if ( !IsWhiteSpace( _buffer[ i ] ) )
                    break;
                length--;
            }
            index = i;
            return length;
        }
        private int TrimEnd( int index, int length ) {
            for( int i = index + length - 1; length > 0; i-- ) {
                if( !IsWhiteSpace( _buffer[ i ] ) )
                    break;
                length--;
            }
            return length;
        }
        private char[] _textChars = new char[ 128 ];

        private int TrimEnd( int length ) {
            for ( int i = length - 1; length > 0; i-- ) {
                if ( !char.IsWhiteSpace( _textChars[ i ] ) )
                    break;
                length--;
            }
            return length;
        }

        private void Add( List<string> segs, int from, int length ) {
            length = TrimStart( ref from, length );
            length = TrimEnd( from, length );
            string val = "";
            if ( length > 0 ) {
                if ( length + 1 > _textChars.Length ) {
                    _textChars = new char[ length + 16 ];
                }
                int charLn = Encoding.UTF8.GetChars( _buffer, from, length, _textChars, 0 );
                charLn = TrimEnd( charLn );
                if ( charLn > 0 ) {
                    val = new string( _textChars, 0, charLn );
                }
            }
            segs.Add( val );
        }

        public int ReadSegments( List<string> segs ) {
            int index;
            int len = GetNextLine( out index );
            if ( len <= 0 ) {
                if ( len == 0 ) {
                    if ( ( _options & TextReaderOptions.KeepEmptyLines ) == 0 ) {
                        return 0;
                    }
                    Add( segs, index, len );
                    return 1;
                }
                return -1;
            }

            if ( _offset > 0 && _buffer[ index + len - 1 ] == '\r' ) {
                --len;
            }
            if ( ( _options & TextReaderOptions.DontTrimLines ) == 0 ) {
                len = TrimStart( ref index, len );
            }
            int idx;
            if ( ( _options & TextReaderOptions.DontCommentLine ) == 0 ) {
                idx = IndexOf( '#', index, len );
                if ( idx >= 0 ) {
                    len = idx - index;
                }
            }

            if ( len == 0 ) {
                if ( ( _options & TextReaderOptions.KeepEmptyLines ) == 0 ) {
                    return 0;
                }
            }
            idx = IndexOf( ';', index, len );
            while ( idx >= 0 ) {
                int segLength = idx - index;
                Add( segs, index, segLength );
                index = idx + 1;
                len -= segLength + 1;
                idx = IndexOf( ';', index, len );
            }
            if( ( _options & TextReaderOptions.SeachCommentAtEnd ) == TextReaderOptions.SeachCommentAtEnd ) {
                idx = IndexOf( '#', index, len );
                if( idx >= 0 ) {
                    len = idx - index;
                }
            }
            Add( segs, index, len );
            return segs.Count;
        }

        public string ReadLine() {
            int index;
            int len = GetNextLine( out index );
            if ( len > 0 ) {
                if ( _offset > 0 && _buffer[ index + len - 1 ] == '\r' ) {
                    if ( --len == 0 )
                        return "";
                }
                return Encoding.UTF8.GetString( _buffer, index, len );
            }
            if ( len == 0 ) {
                return "";
            }
            return null;
        }

        

        private int GetNextLine( out int index ) {
            int len;
            int i = IndexOf( _index );
            if ( i >= 0 ) {
                index = _index;
                len = i - _index;
                _index = i + 1;
                return len;
            }
            i = _length - EmptyBuffer();
            index = 0;

            while ( _stream != null ) {
                len = _buffer.Length - _length;
                if ( len < LineSize ) {
                    Array.Resize( ref _buffer, _buffer.Length * 2 );
                    len = _buffer.Length - _length;
                }
                int ns = _stream.Read( _buffer, _length, len );
                if ( ns == 0 ) {
                    Dispose();
                    break;
                }
                _offset += ns;
                _length += ns;
                i = IndexOf( i );
                if ( i >= 0 ) {
                    len = i - _index;
                    _index = i + 1;
                    return len;
                }
                i = _length;
            }
            if ( _length == 0 ) {
                return -1;
            }
            _index = _length;
            return _length;
        }

        private int EmptyBuffer() {
            if ( _index > 0 ) {
                int len = _length - _index;
                if ( len > 0 ) {
                    Array.Copy( _buffer, _index, _buffer, 0, len );
                }
                _length = len;
                len = _index;
                _index = 0;
                return len;
            }
            return 0;
        }

        public void Dispose() {
            Interlocked.Exchange( ref _stream, null )?.Dispose();
        }
    }
}
