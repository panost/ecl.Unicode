using System;
using ecl.Unicode;

namespace eclUnicode.Cldr.Locale {
    class LanguageInfo : CodeObjectBase {
        /// <summary>
        /// if true is the default locale of it's resource parent
        /// </summary>
        public bool IsDefault;
        public bool HasFile;
        /// <summary>
        /// 
        /// </summary>
        public string Code {
            get {
                return _code;
            }
            set {
                //if ( value.SameName( "ken_CM" ) ) {
                //    Debug.WriteLine( "vvv" );
                //}
                _code = value;
            }
        }
        private LanguageInfo _parent;
        /// <summary>
        /// Resource parent
        /// </summary>
        public LanguageInfo Parent {
            get { return _parent; }
            set { _parent = value; }
        }

        //private LanguageInfo _localeParent;

        //public LanguageInfo GetLocaleParent( CldrLoader loader ) {
        //    if ( _localeParent == null && _code.IndexOf( '_' ) > 0 ) {
        //        _localeParent = loader.GetLocaleParent( _code );
        //    }
        //    return _localeParent;
        //}
        private CldrLocale _locale;
        /// <summary>
        /// 
        /// </summary>
        public CldrLocale Locale {
            get { return _locale; }
            set { _locale = value; }
        }
        private WritingScript[] _scripts;
        /// <summary>
        /// 
        /// </summary>
        public WritingScript[] Scripts {
            get { return _scripts; }
            set { _scripts = value; }
        }

        private WritingScript[] _scripts2;
        /// <summary>
        /// 
        /// </summary>
        public WritingScript[] Scripts2 {
            get { return _scripts2; }
            set { _scripts2 = value; }
        }

        public void AddScripts( WritingScript[] scripts, bool secondary ) {
            if( secondary ) {
                _scripts2 = scripts;
            } else {
                _scripts = scripts;
            }
        }
        private Territory[] _territories;
        /// <summary>
        /// 
        /// </summary>
        public Territory[] Territories {
            get { return _territories; }
            set { _territories = value; }
        }
        private Territory[] _territories2;

        /// <summary>
        /// 
        /// </summary>
        public Territory[] Territories2 {
            get { return _territories2; }
            set { _territories2 = value; }
        }

        internal void AddTerritories( Territory[] territory, bool secondary ) {
            if ( secondary ) {
                _territories2 = territory;
            } else {
                _territories = territory;
            }
        }
    }
}
