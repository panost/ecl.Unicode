using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace ecl.Unicode {
    struct ZipLoader : IDisposable {
        private string _root;
        private ZipArchive _zip;

        public ZipLoader( ZipArchive zip, string rootFolder= null ) {
            _zip = zip;
            _root = rootFolder;
        }

        private static string FixFolder( string root ) {
            if( root.HasValue() ) {
                root = root.Replace( '\\', '/' );
                if( root[ 0 ] == '/' ) {
                    root = root.Substring( 1 );
                }
            }
            if( !root.HasValue() ) {
                return "";
            }
            if( root[ root.Length - 1 ] != '/' ) {
                root += '/';
            }
            return root;
        }
        public ZipLoader( string anyFile, string zipRoot = null ) {
            anyFile = Path.GetFullPath( anyFile );
            if( Directory.Exists( anyFile ) ) {
                if( anyFile[ anyFile.Length - 1 ] != '/' ) {
                    anyFile += '/';
                }
                _root = anyFile;
                _zip = null;
            } else if ( !File.Exists( anyFile ) ) {
                throw new FileNotFoundException( anyFile );
            }
            _zip= ZipFile.OpenRead( anyFile );
            _root = FixFolder( zipRoot );
        }

        public Stream OpenFile( string folder, string name) {
            string fileName;
            return TryOpenFile( folder, name, out fileName );
        }
        public Stream OpenFile( string name ) {
            string fileName;
            return TryOpenFile( "", name, out fileName );
        }
        public Stream TryOpenFile( string folder, string name, out string fileName ) {
            folder = FixFolder( folder );

            if( _zip != null ) {
                fileName = _root + folder + name;
                return _zip.GetEntry( fileName )?.Open();
            }
            if ( _root == null )
                throw new ObjectDisposedException( "ZipLoader" );

            fileName = Path.Combine( _root+ folder, name );
            if ( File.Exists( fileName ) ) {
                return new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
            }
            return null;
        }

        public bool FileExists( string folder, string name ) {
            string fileName;
            if( _zip != null ) {
                fileName = "common/" + folder + "/" + name + ".xml";
                return _zip.GetEntry( fileName ) != null;
            }
            if ( _root == null )
                throw new ObjectDisposedException( "ZipLoader" );

            fileName = Path.Combine( _root, folder, name + ".xml" );
            return File.Exists( fileName );
        }

        public void Dispose() {
            Interlocked.Exchange( ref _zip, null )?.Dispose();
            _root = null;
        }
    }
}
