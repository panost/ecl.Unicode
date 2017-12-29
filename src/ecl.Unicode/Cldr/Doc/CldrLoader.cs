using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Xml;
using ecl.Unicode.Cldr.Locale;

namespace ecl.Unicode.Cldr.Doc {
    public partial class CldrLoader : IDisposable {
        public readonly string Root;
        private ZipArchive _zip;
        //private LdmlDocument LoaDocument2( string name, LdmlDocument parent ) {
        //    string full = Path.Combine( Root, "main", name + ".xml" );
        //    return LdmlDocument.Load( this, full, parent );
        //}
        readonly XmlReaderSettings _xmlSettings = new XmlReaderSettings {
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Ignore
        };

        //private readonly Dictionary<string, CldrLocale> _loadedLocales =
        //    new Dictionary<string, CldrLocale>( StringComparer.OrdinalIgnoreCase );

        public CldrLocale RootLocale {
            get {
                return GetLocale( "root" );
            }
        }

        public CldrLocale GetLocale( string localeName ) {
            LanguageInfo localeInfo = FindLanguage( ref localeName );
            if ( localeInfo?.Locale != null ) {
                return localeInfo.Locale;
            }
            CldrLocale rootLocale = GetLocaleInfo( "root" ).Locale;
            if( rootLocale == null ) {
                rootLocale = TryLoadLanguage( "root", null );
                if ( localeName.Equals( "root", StringComparison.OrdinalIgnoreCase ) ) {
                    return rootLocale;
                }
            }
            CldrLocale parentLocale = null;
            if ( localeInfo == null ) {
                localeInfo = GetLocaleInfo( localeName );
            }
            LanguageInfo parentInfo = localeInfo.Parent;
            while ( parentInfo == null ) {
                int idx = localeName.LastIndexOf( '_' );
                if ( idx > 0 ) {
                    parentInfo = ResolveLanguage( localeName.Substring( 0, idx ) );
                } else {
                    break;
                }
            }
            if ( parentInfo != null ) {
                parentLocale = GetLocale( parentInfo.Code );
            }
            return TryLoadLanguage( localeName, parentLocale ?? rootLocale );
        }
        private CldrLocale TryLoadLanguage( string name, CldrLocale parent ) {
            XmlReader reader = TryOpenXmlFile( "main", name );
            if ( reader == null ) {
                return null;
            }
            CldrLocale doc;
            try {
                doc = new CldrLocale( this, parent, ResolveLanguage( name) );
                doc.LoadMain( reader );
            } finally {
                reader.Dispose();
            }
            if ( ( LoadOptions & CldrLoadOptions.Rbnf ) != 0 ) {
                reader = TryOpenXmlFile( "rbnf", name );
                if ( reader != null ) {
                    try {
                        doc.LoadMisc( reader, "rbnf" );
                    } finally {
                        reader.Dispose();
                    }
                }
            }
            doc.Loaded();
            if ( _descriptionsProvider != null ) {
                doc.ResolveDescription( _descriptionsProvider );
            }
            return doc;
        }

        //readonly HashSet<string> _allLocaleNames = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
        private CldrLocale[] _allLoaded;

        public void EnsureAllLocalesLoaded() {
            if ( _allLoaded == null )
                return;
            foreach ( string name in GetAllLocaleNames() ) {
                GetLocale( name );
            }
            Dictionary<CldrLocale,List<CldrLocale>> map = new Dictionary<CldrLocale, List<CldrLocale>>();
            List<CldrLocale> list = new List<CldrLocale>( _localeInfo.Count );
            foreach ( LanguageInfo info in _localeInfo.Values ) {
                CldrLocale locale = info.Locale;
                if ( locale != null ) {
                    list.Add( locale );
                    var parent = locale.Parent;
                    if( parent != null ) {
                        map.AddGroup( parent, locale );
                    }
                    locale.Children = CldrLocale.EmptyArray;
                } else {
                    Debug.WriteLine( "W+"+ info.Code );
                }
            }
            _allLoaded = list.ToArray();
            foreach ( var pair in map ) {
                pair.Key.Children = pair.Value.ToArray();
            }
        }
        public IEnumerable<CldrLocale> GetCultures() {
            EnsureAllLocalesLoaded();
            EnsureEnglishNames();
            return _allLoaded;
        }

        private IEnumerable<string> GetAllLocaleNames() {
            if ( _zip != null ) {
                const string MainFolder = "common/main/";
                foreach ( ZipArchiveEntry entry in _zip.Entries ) {
                    var name = entry.FullName;
                    if ( name.StartsWith( MainFolder, StringComparison.OrdinalIgnoreCase )
                        && name.EndsWith( ".xml", StringComparison.OrdinalIgnoreCase ) ) {
                        int length = name.Length;
                        name = name.Substring( MainFolder.Length, length - MainFolder.Length - 4 );
                        yield return name;
                    }
                }
            } else {
                DirectoryInfo mainFolder = new DirectoryInfo( Path.Combine( Root, "main" ) );
                foreach ( var file in mainFolder.GetFiles( "*.xml", SearchOption.TopDirectoryOnly ) ) {
                    yield return Path.GetFileNameWithoutExtension( file.Name );
                }
            }
        }

        private bool FileExists( string folder, string name ) {
            string fileName;
            if ( _zip != null ) {
                fileName = "common/" + folder + "/" + name + ".xml";
                return _zip.GetEntry( fileName ) != null;
            }
            fileName = Path.Combine( Root, folder, name + ".xml" );
            return File.Exists( fileName );
        }
        private Stream TryOpenFile( string folder, string name, out string fileName ) {
            if ( _zip != null ) {
                fileName = "common/" + folder + "/" + name + ".xml";
                ZipArchiveEntry zip = _zip.GetEntry( fileName );
                if ( zip != null ) {
                    return zip.Open();
                }
            } else {
                fileName = Path.Combine( Root, folder, name + ".xml" );
                if ( File.Exists( fileName ) ) {
                    return new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
                }
            }
            return null;
        }
        internal XmlReader TryOpenXmlFile( string folder, string name ) {
            string fileName;
            Stream stream = TryOpenFile( folder, name, out fileName );
            if ( stream != null ) {
                return XmlReader.Create( stream, _xmlSettings );
            }
            return null;
        }
        private XmlReader OpenXmlFile( string folder, string name ) {
            return XmlReader.Create( OpenFile( folder, name ), _xmlSettings );
        }
        private Stream OpenFile( string folder, string name ) {
            string fileName;
            Stream stream = TryOpenFile( folder, name, out fileName );
            if ( stream != null )
                return stream;
            throw new FileNotFoundException( fileName );
        }

        public readonly CldrLoadOptions LoadOptions;

        public CldrLoader( string rootPath, CldrLoadOptions loadOptions = CldrLoadOptions.Normal ) {
            LoadOptions = loadOptions;
            if ( File.Exists( rootPath ) ) {
                _zip = ZipFile.OpenRead( rootPath );
            } else {
                DirectoryInfo info = new DirectoryInfo( rootPath );
                if ( !info.Exists ) {
                    throw new DirectoryNotFoundException( info.FullName );
                }
                rootPath = info.FullName;
                if ( !Directory.Exists( Path.Combine( rootPath, "main" ) ) ) {
                    rootPath = Path.Combine( rootPath, "common" );
                    if ( !Directory.Exists( Path.Combine( rootPath, "main" ) ) ) {
                        throw new FileLoadException( "Not a valid CLDR folder" );
                    }
                }
                Root = rootPath;
            }
            _scripts = XUtil.GetScriptMap();
            MetaDataLoader loader = new MetaDataLoader( this );

            using ( XmlReader reader = OpenXmlFile( "supplemental", "supplementalMetadata" ) ) {
                loader.LoadMetaData( reader );
            }
            using ( XmlReader reader = OpenXmlFile( "supplemental", "supplementalData" ) ) {
                loader.Load( reader );
            }
            foreach ( string name in GetAllLocaleNames() ) {
                GetLocaleInfo( name ).HasFile = true;
            }
        }
        private Calendar[] _defaultCalendars;
        /// <summary>
        /// 
        /// </summary>
        internal Calendar[] DefaultCalendars {
            get { return _defaultCalendars; }
            set { _defaultCalendars = value; }
        }

        public Calendar DefaultCalendar {
            get {
                return _defaultCalendars?[ 0 ];
            }
        }
        private readonly Dictionary<string, Calendar> _calendars =
            new Dictionary<string, Calendar>( StringComparer.OrdinalIgnoreCase );

        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, Calendar> Calendars {
            get { return _calendars; }
        }
        private Calendar GetOrCreateCalendar( string name ) {
            Calendar calendar = FindCalendar( name );
            if( calendar == null ) {
                calendar = new Calendar() {
                    Name = name
                };
                _calendars.Add( name, calendar );
            }
            return calendar;
        }

        public Dictionary<string, Calendar>.ValueCollection AllCalendars {
            get { return _calendars.Values; }
        }

        public Calendar FindCalendar( string name ) {
            return _calendars.GetValueOrDefault( name );
        }
        public Calendar GetCalendar( string name ) {
            Calendar calendar = FindCalendar( name );
            if ( calendar == null ) {
                throw new ArgumentException("Unable to find calendar " + name);
            }
            return calendar;
        }

        private readonly Dictionary<string, Calendar[]> _calendarPreferences =
            new Dictionary<string, Calendar[]>( StringComparer.OrdinalIgnoreCase );

        public Calendar[] GetCalendarPreference( string territory ) {
            Calendar[] cals;
            if ( !territory.HasValue()
                 || !_calendarPreferences.TryGetValue( territory, out cals ) ) {
                return _defaultCalendars;
            }
            return cals;
        }

        private CldrLocale _descriptionsProvider;

        public void EnsureEnglishNames() {
            EnsureDescriptions( GetLocale( "en" ) );
        }

        public void EnsureDescriptions( CldrLocale provider ) {
            if ( _descriptionsProvider == provider || provider == null )
                return;
            _descriptionsProvider = provider;
            var names = provider.LocaleDisplayNames.TerritoryNames;
            foreach ( Territory ter in _territories.Values ) {
                string name;
                if ( names.TryGetValue( ter, out name ) ) {
                    ter.Description = name;
                }
            }
            foreach ( LanguageInfo info in _localeInfo.Values ) {
                info.Locale?.ResolveDescription( provider );
            }
        }
        private readonly Dictionary<string, string> _ldmlNames = new Dictionary<string, string>( StringComparer.Ordinal );
        private Dictionary<string, WritingScript> _scripts;

        internal string GetCachedName( string name ) {
            string iname = string.IsInterned( name );
            if ( iname == null ) {
                if ( !_ldmlNames.TryGetValue( name, out iname ) ) {
                    _ldmlNames.Add( name, name );
                    iname = name;
                }
            }
            return iname;
        }

        public void Warning( string msg ) {
            Debug.WriteLine( msg );
        }

        public void Dispose() {
            var zip = Interlocked.Exchange( ref _zip, null );
            if ( zip != null ) {
                zip.Dispose();
            }
        }
        protected virtual void Error( string format, params object[] args ) {
            throw new InvalidDataException( string.Format( format, args ) );
        }

        public WritingScript GetScript( string code ) {
            WritingScript value;
            if ( !_scripts.TryGetValue( code, out value ) ) {
                Error( $"Unable to find script '{code}'" );
                return 0;
            }
            return value;
        }

        public Territory FindTerritory( string code ) {
            return _territories.GetValueOrDefault( code );
        }
        public Territory GetTerritory( string code ) {
            return _territories[ code ];
        }
    }
}
