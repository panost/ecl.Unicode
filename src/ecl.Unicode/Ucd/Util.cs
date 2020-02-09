using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ecl.Unicode.Ucd {
    public static class Util {
        internal static Dictionary<int, string> GetAlternativeNames() {
            Dictionary<int, string> map = new Dictionary<int, string>();
            map.Add( 0x80, "Padding Character" );
            map.Add( 0x81, "High Octet Preset" );
            map.Add( 0x84, "Index" );
            map.Add( 0x99, "Single Graphic Character Introducer" );
            map.Add( 0x1f514, "Emoji of Bell" );
            return map;
        }
        public static string GenEnumName( string name ) {
            StringBuilder b = new StringBuilder();
            var length = name.Length;
            var upper = true;
            for( int i = 0; i < length; i++ ) {
                char ch = name[ i ];
                if( char.IsLetter( ch ) || ( b.Length > 0 && char.IsDigit( ch ) ) ) {
                    if( upper ) {
                        upper = false;
                        b.Append( char.ToUpperInvariant( ch ) );
                    } else {
                        b.Append( char.ToLowerInvariant( ch ) );
                    }
                } else {
                    upper = true;
                    if( ch == '-' && b.Length > 0 ) {
                        b.Append( '_' );
                    }
                    if( ch == '(' ) {
                        break;
                    }
                }
            }

            return b.ToString();
        }
        public static IEnumerable<int> GetLines( this LineReader r, List<string> segs, int minSegs = 0 ) {
            while ( true ) {
                segs.Clear();
                int count = r.ReadSegments( segs );

                if ( count >= minSegs ) {
                    yield return count;
                } else if ( count < 0 ) {
                    break;
                }
            }
        }

        public static IEnumerable<int> GetLines( Stream str, List<string> segs, TextReaderOptions options ) {
            using ( LineReader r = new LineReader( str, options ) ) {
                return r.GetLines( segs, 1 );
            }
        }

        // http://unicode.org/reports/tr9/#Bidirectional_Character_Types
        internal static Dictionary<string, BidirectionalCategory> GetBidiMap() {
            var map = new Dictionary<string, BidirectionalCategory>( StringComparer.OrdinalIgnoreCase );
            map.Add( "L", BidirectionalCategory.LeftToRight );
            map.Add( "R", BidirectionalCategory.RightToLeft );
            map.Add( "AL", BidirectionalCategory.ArabicLetter );

            map.Add( "EN", BidirectionalCategory.EuropeanNumber );
            map.Add( "ES", BidirectionalCategory.EuropeanSeparator );
            map.Add( "ET", BidirectionalCategory.EuropeanTerminator );
            map.Add( "AN", BidirectionalCategory.ArabicNumber );
            map.Add( "CS", BidirectionalCategory.CommonSeparator );
            map.Add( "NSM", BidirectionalCategory.NonSpacingMark );
            map.Add( "BN", BidirectionalCategory.BoundaryNeutral );

            map.Add( "B", BidirectionalCategory.ParagraphSeparator );
            map.Add( "S", BidirectionalCategory.SegmentSeparator );
            map.Add( "WS", BidirectionalCategory.WhiteSpace );
            map.Add( "ON", BidirectionalCategory.OtherNeutral );

            map.Add( "LRE", BidirectionalCategory.LeftToRightEmbedding );
            map.Add( "LRO", BidirectionalCategory.LeftToRightOverride );
            map.Add( "RLE", BidirectionalCategory.RightToLeftEmbedding );
            map.Add( "RLO", BidirectionalCategory.RightToLeftOverride );
            map.Add( "PDF", BidirectionalCategory.PopDirectionalFormat );

            map.Add( "LRI", BidirectionalCategory.LeftToRightIsolate );
            map.Add( "RLI", BidirectionalCategory.RightToLeftIsolate );
            map.Add( "FSI", BidirectionalCategory.FirstStrongIsolate );
            map.Add( "PDI", BidirectionalCategory.PopDirectionalIsolate );

            map.Add( "Arabic_Letter", BidirectionalCategory.ArabicLetter );
            map.Add( "Arabic_Number", BidirectionalCategory.ArabicNumber );
            map.Add( "Paragraph_Separator", BidirectionalCategory.ParagraphSeparator );
            map.Add( "Boundary_Neutral", BidirectionalCategory.BoundaryNeutral );
            map.Add( "Common_Separator", BidirectionalCategory.CommonSeparator );
            map.Add( "European_Number", BidirectionalCategory.EuropeanNumber );
            map.Add( "European_Separator", BidirectionalCategory.EuropeanSeparator );
            map.Add( "European_Terminator", BidirectionalCategory.EuropeanTerminator );
            map.Add( "First_Strong_Isolate", BidirectionalCategory.FirstStrongIsolate );
            map.Add( "Left_To_Right", BidirectionalCategory.LeftToRight );
            map.Add( "Left_To_Right_Embedding", BidirectionalCategory.LeftToRightEmbedding );
            map.Add( "Left_To_Right_Isolate", BidirectionalCategory.LeftToRightIsolate );
            map.Add( "Left_To_Right_Override", BidirectionalCategory.LeftToRightOverride );
            map.Add( "Nonspacing_Mark", BidirectionalCategory.NonSpacingMark );
            map.Add( "Other_Neutral", BidirectionalCategory.OtherNeutral );
            map.Add( "Pop_Directional_Format", BidirectionalCategory.PopDirectionalFormat );
            map.Add( "Pop_Directional_Isolate", BidirectionalCategory.PopDirectionalIsolate );
            map.Add( "Right_To_Left", BidirectionalCategory.RightToLeft );
            map.Add( "Right_To_Left_Embedding", BidirectionalCategory.RightToLeftEmbedding );
            map.Add( "Right_To_Left_Isolate", BidirectionalCategory.RightToLeftIsolate );
            map.Add( "Right_To_Left_Override", BidirectionalCategory.RightToLeftOverride );
            map.Add( "Segment_Separator", BidirectionalCategory.SegmentSeparator );
            map.Add( "White_Space", BidirectionalCategory.WhiteSpace );

            return map;
        }

        private static readonly string[] _unicodeProperties = {
            null,
            "ASCII_Hex_Digit", // CodePointProperty.ASCII_Hex_Digit
            "Bidi_Control", // CodePointProperty.Bidi_Control
            "Dash", // CodePointProperty.Dash
            "Deprecated", // CodePointProperty.Deprecated
            "Diacritic", // CodePointProperty.Diacritic
            "Extender", // CodePointProperty.Extender
            "Hex_Digit", // CodePointProperty.Hex_Digit
            "Hyphen", // CodePointProperty.Hyphen
            "Ideographic", // CodePointProperty.Ideographic
            "IDS_Binary_Operator", // CodePointProperty.IDS_Binary_Operator
            "IDS_Trinary_Operator", // CodePointProperty.IDS_Trinary_Operator
            "Join_Control", // CodePointProperty.Join_Control
            "Logical_Order_Exception", // CodePointProperty.Logical_Order_Exception
            "Noncharacter_Code_Point", // CodePointProperty.Noncharacter_Code_Point
            "Other_Alphabetic", // CodePointProperty.Other_Alphabetic
            "Other_Default_Ignorable_Code_Point", // CodePointProperty.Other_Default_Ignorable_Code_Point
            "Other_Grapheme_Extend", // CodePointProperty.Other_Grapheme_Extend
            "Other_ID_Continue", // CodePointProperty.Other_ID_Continue
            "Other_ID_Start", // CodePointProperty.Other_ID_Start
            "Other_Lowercase", // CodePointProperty.Other_Lowercase
            "Other_Math", // CodePointProperty.Other_Math
            "Other_Uppercase", // CodePointProperty.Other_Uppercase
            "Pattern_Syntax", // CodePointProperty.Pattern_Syntax
            "Quotation_Mark", // CodePointProperty.Quotation_Mark
            "Radical", // CodePointProperty.Radical
            "Soft_Dotted", // CodePointProperty.Soft_Dotted
            "STerm", // CodePointProperty.STerm
            "Terminal_Punctuation", // CodePointProperty.Terminal_Punctuation
            "Unified_Ideograph", // CodePointProperty.Unified_Ideograph
            "Variation_Selector", // CodePointProperty.Variation_Selector
            "White_Space", // CodePointProperty.White_Space

        };
        public static string GetCode( this CodePointProperty property ) {
            return _unicodeProperties[ (int)property ];
        }
        internal static Dictionary<string, CodePointProperty> GetPropertyMap() {
            var map = new Dictionary<string, CodePointProperty>( StringComparer.OrdinalIgnoreCase );
            for( int i = 1; i < _unicodeProperties.Length; i++ ) {
                map.Add( _unicodeProperties[ i ], (CodePointProperty)i );
            }
            return map;
        }
        private static readonly string[] _unicodeCharacterCodes = {
            "Lu", // UnicodeCharacterType.LetterUppercase
            "Ll", // UnicodeCharacterType.LetterLowercase
            "Lt", // UnicodeCharacterType.LetterTitlecase
            "Lm", // UnicodeCharacterType.LetterModifier
            "Lo", // UnicodeCharacterType.LetterOther

            "Mn", // UnicodeCharacterType.MarkNonSpacing
            "Mc", // UnicodeCharacterType.MarkSpacingCombining
            "Me", // UnicodeCharacterType.MarkEnclosing

            "Nd", // UnicodeCharacterType.NumberDecimalDigit
            "Nl", // UnicodeCharacterType.NumberLetter
            "No", // UnicodeCharacterType.NumberOther

            "Zs", // UnicodeCharacterType.SeparatorSpace
            "Zl", // UnicodeCharacterType.SeparatorLine
            "Zp", // UnicodeCharacterType.SeparatorParagraph

            "Cc", // UnicodeCharacterType.OtherControl
            "Cf", // UnicodeCharacterType.OtherFormat
            "Cs", // UnicodeCharacterType.OtherSurrogate
            "Co", // UnicodeCharacterType.OtherPrivateUse
            "Cn", // UnicodeCharacterType.OtherNotAssigned

            "Pc", // UnicodeCharacterType.PunctuationConnector
            "Pd", // UnicodeCharacterType.PunctuationDash
            "Ps", // UnicodeCharacterType.PunctuationOpen
            "Pe", // UnicodeCharacterType.PunctuationClose
            "Pi", // UnicodeCharacterType.PunctuationInitialQuote
            "Pf", // UnicodeCharacterType.PunctuationFinalQuote
            "Po", // UnicodeCharacterType.PunctuationOther
            
            "Sm", // UnicodeCharacterType.SymbolMath
            "Sc", // UnicodeCharacterType.SymbolCurrency
            "Sk", // UnicodeCharacterType.SymbolModifier
            "So" // UnicodeCharacterType.SymbolOther
        };

        public static string GetCode( this UnicodeCharacterType type ) {
            return _unicodeCharacterCodes[ (int)type ];
        }
        public static char FirstCode( this UnicodeCharacterType type ) {
            return _unicodeCharacterCodes[ (int)type ][0];
        }
        public static bool IsLetter( this UnicodeCharacterType type ) {
            return FirstCode( type ) == 'L';
        }
        internal static Dictionary<string, UnicodeCharacterType> GetTypeMap() {
            var map = new Dictionary<string, UnicodeCharacterType>( StringComparer.OrdinalIgnoreCase );
            for ( int i = 0; i < _unicodeCharacterCodes.Length; i++ ) {
                map.Add( _unicodeCharacterCodes[ i ], (UnicodeCharacterType)i );
            }
            return map;
        }

        internal static Dictionary<string, DecomposingTag> GetDecombosingMap() {
            var map = new Dictionary<string, DecomposingTag>( StringComparer.OrdinalIgnoreCase );
            map.Add( "<font>", DecomposingTag.Font );
            map.Add( "<noBreak>", DecomposingTag.NoBreak );
            map.Add( "<initial>", DecomposingTag.Initial );
            map.Add( "<medial>", DecomposingTag.Medial );
            map.Add( "<final>", DecomposingTag.Final );
            map.Add( "<isolated>", DecomposingTag.Isolated );
            map.Add( "<circle>", DecomposingTag.Circle );
            map.Add( "<super>", DecomposingTag.Super );
            map.Add( "<sub>", DecomposingTag.Sub );
            map.Add( "<vertical>", DecomposingTag.Vertical );
            map.Add( "<wide>", DecomposingTag.Wide );
            map.Add( "<narrow>", DecomposingTag.Narrow );
            map.Add( "<small>", DecomposingTag.Small );
            map.Add( "<square>", DecomposingTag.Square );
            map.Add( "<fraction>", DecomposingTag.Fraction );
            map.Add( "<compat>", DecomposingTag.Compat );


            return map;
        }

        private static readonly Dictionary<string, GraphemeClusterBreak> _graphemeClusterBreakMap = GetGraphemeClusterBreak();

        private static Dictionary<string, GraphemeClusterBreak> GetGraphemeClusterBreak() {
            return new Dictionary<string, GraphemeClusterBreak>( StringComparer.OrdinalIgnoreCase ) {
                { "Prepend", GraphemeClusterBreak.Prepend },
                { "CR", GraphemeClusterBreak.CarriageReturn },
                { "LF", GraphemeClusterBreak.LineFeed },
                { "Control", GraphemeClusterBreak.Control },
                { "Extend", GraphemeClusterBreak.Extend },
                { "Regional_Indicator", GraphemeClusterBreak.RegionalIndicator },
                { "SpacingMark", GraphemeClusterBreak.SpacingMark },
                { "L", GraphemeClusterBreak.L },
                { "V", GraphemeClusterBreak.V },
                { "T", GraphemeClusterBreak.T },
                { "LV", GraphemeClusterBreak.LV },
                { "LVT", GraphemeClusterBreak.LVT },
                { "ZWJ", GraphemeClusterBreak.ZeroWidthJoiner }
            };
        }

        private static readonly Dictionary<string, WordBreak> _wordBreakMap = GetWordBreaks();

        private static Dictionary<string, WordBreak> GetWordBreaks() {
            return new Dictionary<string, WordBreak>( StringComparer.OrdinalIgnoreCase ) {
                { "Double_Quote", WordBreak.DoubleQuote },
                { "Single_Quote", WordBreak.SingleQuote },
                { "Hebrew_Letter", WordBreak.HebrewLetter },
                { "CR", WordBreak.Cr },
                { "LF", WordBreak.Lf },
                { "Newline", WordBreak.Newline },
                { "Extend", WordBreak.Extend },
                { "Regional_Indicator", WordBreak.RegionalIndicator },
                { "Format", WordBreak.Format },
                { "Katakana", WordBreak.Katakana },
                { "ALetter", WordBreak.ALetter },
                { "MidLetter", WordBreak.MidLetter },
                { "MidNum", WordBreak.MidNum },
                { "MidNumLet", WordBreak.MidNumLet },
                { "Numeric", WordBreak.Numeric },
                { "ExtendNumLet", WordBreak.ExtendNumLet },
                { "ZWJ", WordBreak.ZeroWidthJoiner },
                { "WSegSpace", WordBreak.WSegSpace }
            };
        }

        private static readonly Dictionary<string, SentenceBreak> _sentenceBreakMap = GetSentenceBreaks();

        private static Dictionary<string, SentenceBreak> GetSentenceBreaks() {
            return new Dictionary<string, SentenceBreak>( StringComparer.OrdinalIgnoreCase ) {
                { "CR", SentenceBreak.CR },
                { "LF", SentenceBreak.LF },
                { "Extend", SentenceBreak.Extend },
                { "Sep", SentenceBreak.Sep },
                { "Format", SentenceBreak.Format },
                { "Sp", SentenceBreak.Sp },
                { "Lower", SentenceBreak.Lower },
                { "Upper", SentenceBreak.Upper },
                { "OLetter", SentenceBreak.OLetter },
                { "Numeric", SentenceBreak.Numeric },
                { "ATerm", SentenceBreak.ATerm },
                { "STerm", SentenceBreak.STerm },
                { "Close", SentenceBreak.Close },
                { "SContinue", SentenceBreak.SContinue },
            };
        }
        internal static GraphemeClusterBreak ParseGraphemeClusterBreak( string text ) {
            if (!_graphemeClusterBreakMap.TryGetValue( text, out var result ))
                return 0;
            return result;
        }
        internal static SentenceBreak ParseSentenceBreak( string text ) {
            if (!_sentenceBreakMap.TryGetValue( text, out var result ))
                return 0;
            return result;
        }
        internal static WordBreak ParseWordBreak( string text ) {
            if (!_wordBreakMap.TryGetValue( text, out var result ))
                return 0;
            return result;
        }

        //internal static Dictionary<string, Script> GetScriptsMap() {
        //    var map = new Dictionary<string, Script>( StringComparer.OrdinalIgnoreCase );

        //    map.Add( "Aghb", Script.CaucasianAlbanian );
        //    //map.Add( "Ahom", Script.Ahom );
        //    map.Add( "Arab", Script.Arabic );
        //    map.Add( "Armi", Script.ImperialAramaic );
        //    map.Add( "Armn", Script.Armenian );
        //    map.Add( "Avst", Script.Avestan );
        //    map.Add( "Bali", Script.Balinese );
        //    map.Add( "Bamu", Script.Bamum );
        //    map.Add( "Bass", Script.BassaVah );
        //    map.Add( "Batk", Script.Batak );
        //    map.Add( "Beng", Script.Bengali );
        //    map.Add( "Bopo", Script.Bopomofo );
        //    map.Add( "Brah", Script.Brahmi );
        //    map.Add( "Brai", Script.Braille );
        //    map.Add( "Bugi", Script.Buginese );
        //    map.Add( "Buhd", Script.Buhid );
        //    map.Add( "Cakm", Script.Chakma );
        //    map.Add( "Cans", Script.CanadianAboriginal );
        //    map.Add( "Cari", Script.Carian );
        //    //map.Add( "Cham", Script.Cham );
        //    map.Add( "Cher", Script.Cherokee );
        //    map.Add( "Copt", Script.Coptic );
        //    map.Add( "Cprt", Script.Cypriot );
        //    map.Add( "Cyrl", Script.Cyrillic );
        //    map.Add( "Deva", Script.Devanagari );
        //    map.Add( "Dsrt", Script.Deseret );
        //    map.Add( "Dupl", Script.Duployan );
        //    map.Add( "Egyp", Script.EgyptianHieroglyphs );
        //    map.Add( "Elba", Script.Elbasan );
        //    map.Add( "Ethi", Script.Ethiopic );
        //    map.Add( "Geor", Script.Georgian );
        //    map.Add( "Glag", Script.Glagolitic );
        //    map.Add( "Goth", Script.Gothic );
        //    map.Add( "Gran", Script.Grantha );
        //    map.Add( "Grek", Script.Greek );
        //    map.Add( "Gujr", Script.Gujarati );
        //    map.Add( "Guru", Script.Gurmukhi );
        //    map.Add( "Hang", Script.Hangul );
        //    map.Add( "Hani", Script.Han );
        //    map.Add( "Hano", Script.Hanunoo );
        //    map.Add( "Hatr", Script.Hatran );
        //    map.Add( "Hebr", Script.Hebrew );
        //    map.Add( "Hira", Script.Hiragana );
        //    map.Add( "Hluw", Script.AnatolianHieroglyphs );
        //    map.Add( "Hmng", Script.PahawhHmong );
        //    map.Add( "Hrkt", Script.KatakanaOrHiragana );
        //    map.Add( "Hung", Script.OldHungarian );
        //    map.Add( "Ital", Script.OldItalic );
        //    map.Add( "Java", Script.Javanese );
        //    map.Add( "Kali", Script.KayahLi );
        //    map.Add( "Kana", Script.Katakana );
        //    map.Add( "Khar", Script.Kharoshthi );
        //    map.Add( "Khmr", Script.Khmer );
        //    map.Add( "Khoj", Script.Khojki );
        //    map.Add( "Knda", Script.Kannada );
        //    map.Add( "Kthi", Script.Kaithi );
        //    map.Add( "Lana", Script.TaiTham );
        //    map.Add( "Laoo", Script.Lao );
        //    map.Add( "Latn", Script.Latin );
        //    map.Add( "Lepc", Script.Lepcha );
        //    map.Add( "Limb", Script.Limbu );
        //    map.Add( "Lina", Script.LinearA );
        //    map.Add( "Linb", Script.LinearB );
        //    //map.Add( "Lisu", Script.Lisu );
        //    map.Add( "Lyci", Script.Lycian );
        //    map.Add( "Lydi", Script.Lydian );
        //    map.Add( "Mahj", Script.Mahajani );
        //    map.Add( "Mand", Script.Mandaic );
        //    map.Add( "Mani", Script.Manichaean );
        //    map.Add( "Mend", Script.MendeKikakui );
        //    map.Add( "Merc", Script.MeroiticCursive );
        //    map.Add( "Mero", Script.MeroiticHieroglyphs );
        //    map.Add( "Mlym", Script.Malayalam );
        //    //map.Add( "Modi", Script.Modi );
        //    map.Add( "Mong", Script.Mongolian );
        //    map.Add( "Mroo", Script.Mro );
        //    map.Add( "Mtei", Script.MeeteiMayek );
        //    map.Add( "Mult", Script.Multani );
        //    map.Add( "Mymr", Script.Myanmar );
        //    map.Add( "Narb", Script.OldNorthArabian );
        //    map.Add( "Nbat", Script.Nabataean );
        //    map.Add( "Nkoo", Script.Nko );
        //    map.Add( "Ogam", Script.Ogham );
        //    map.Add( "Olck", Script.OlChiki );
        //    map.Add( "Orkh", Script.OldTurkic );
        //    map.Add( "Orya", Script.Oriya );
        //    map.Add( "Osma", Script.Osmanya );
        //    map.Add( "Palm", Script.Palmyrene );
        //    map.Add( "Pauc", Script.PauCinHau );
        //    map.Add( "Perm", Script.OldPermic );
        //    map.Add( "Phag", Script.PhagsPa );
        //    map.Add( "Phli", Script.InscriptionalPahlavi );
        //    map.Add( "Phlp", Script.PsalterPahlavi );
        //    map.Add( "Phnx", Script.Phoenician );
        //    map.Add( "Plrd", Script.Miao );
        //    map.Add( "Prti", Script.InscriptionalParthian );
        //    map.Add( "Rjng", Script.Rejang );
        //    map.Add( "Runr", Script.Runic );
        //    map.Add( "Samr", Script.Samaritan );
        //    map.Add( "Sarb", Script.OldSouthArabian );
        //    map.Add( "Saur", Script.Saurashtra );
        //    map.Add( "Sgnw", Script.SignWriting );
        //    map.Add( "Shaw", Script.Shavian );
        //    map.Add( "Shrd", Script.Sharada );
        //    map.Add( "Sidd", Script.Siddham );
        //    map.Add( "Sind", Script.Khudawadi );
        //    map.Add( "Sinh", Script.Sinhala );
        //    map.Add( "Sora", Script.SoraSompeng );
        //    map.Add( "Sund", Script.Sundanese );
        //    map.Add( "Sylo", Script.SylotiNagri );
        //    map.Add( "Syrc", Script.Syriac );
        //    map.Add( "Tagb", Script.Tagbanwa );
        //    map.Add( "Takr", Script.Takri );
        //    map.Add( "Tale", Script.TaiLe );
        //    map.Add( "Talu", Script.NewTaiLue );
        //    map.Add( "Taml", Script.Tamil );
        //    map.Add( "Tavt", Script.TaiViet );
        //    map.Add( "Telu", Script.Telugu );
        //    map.Add( "Tfng", Script.Tifinagh );
        //    map.Add( "Tglg", Script.Tagalog );
        //    map.Add( "Thaa", Script.Thaana );
        //    //map.Add( "Thai", Script.Thai );
        //    map.Add( "Tibt", Script.Tibetan );
        //    map.Add( "Tirh", Script.Tirhuta );
        //    map.Add( "Ugar", Script.Ugaritic );
        //    map.Add( "Vaii", Script.Vai );
        //    map.Add( "Wara", Script.WarangCiti );
        //    map.Add( "Xpeo", Script.OldPersian );
        //    map.Add( "Xsux", Script.Cuneiform );
        //    map.Add( "Yiii", Script.Yi );
        //    map.Add( "Zinh", Script.Inherited );
        //    map.Add( "Zyyy", Script.Common );
        //    map.Add( "Zzzz", Script.Unknown );

        //    map.Add( "Caucasian_Albanian", Script.CaucasianAlbanian );
        //    map.Add( "Ahom", Script.Ahom );
        //    map.Add( "Arabic", Script.Arabic );
        //    map.Add( "Imperial_Aramaic", Script.ImperialAramaic );
        //    map.Add( "Armenian", Script.Armenian );
        //    map.Add( "Avestan", Script.Avestan );
        //    map.Add( "Balinese", Script.Balinese );
        //    map.Add( "Bamum", Script.Bamum );
        //    map.Add( "Bassa_Vah", Script.BassaVah );
        //    map.Add( "Batak", Script.Batak );
        //    map.Add( "Bengali", Script.Bengali );
        //    map.Add( "Bopomofo", Script.Bopomofo );
        //    map.Add( "Brahmi", Script.Brahmi );
        //    map.Add( "Braille", Script.Braille );
        //    map.Add( "Buginese", Script.Buginese );
        //    map.Add( "Buhid", Script.Buhid );
        //    map.Add( "Chakma", Script.Chakma );
        //    map.Add( "Canadian_Aboriginal", Script.CanadianAboriginal );
        //    map.Add( "Carian", Script.Carian );
        //    map.Add( "Cham", Script.Cham );
        //    map.Add( "Cherokee", Script.Cherokee );
        //    map.Add( "Coptic", Script.Coptic );
        //    map.Add( "Qaac", Script.Coptic );
        //    map.Add( "Cypriot", Script.Cypriot );
        //    map.Add( "Cyrillic", Script.Cyrillic );
        //    map.Add( "Devanagari", Script.Devanagari );
        //    map.Add( "Deseret", Script.Deseret );
        //    map.Add( "Duployan", Script.Duployan );
        //    map.Add( "Egyptian_Hieroglyphs", Script.EgyptianHieroglyphs );
        //    map.Add( "Elbasan", Script.Elbasan );
        //    map.Add( "Ethiopic", Script.Ethiopic );
        //    map.Add( "Georgian", Script.Georgian );
        //    map.Add( "Glagolitic", Script.Glagolitic );
        //    map.Add( "Gothic", Script.Gothic );
        //    map.Add( "Grantha", Script.Grantha );
        //    map.Add( "Greek", Script.Greek );
        //    map.Add( "Gujarati", Script.Gujarati );
        //    map.Add( "Gurmukhi", Script.Gurmukhi );
        //    map.Add( "Hangul", Script.Hangul );
        //    map.Add( "Han", Script.Han );
        //    map.Add( "Hanunoo", Script.Hanunoo );
        //    map.Add( "Hatran", Script.Hatran );
        //    map.Add( "Hebrew", Script.Hebrew );
        //    map.Add( "Hiragana", Script.Hiragana );
        //    map.Add( "Anatolian_Hieroglyphs", Script.AnatolianHieroglyphs );
        //    map.Add( "Pahawh_Hmong", Script.PahawhHmong );
        //    map.Add( "Katakana_Or_Hiragana", Script.KatakanaOrHiragana );
        //    map.Add( "Old_Hungarian", Script.OldHungarian );
        //    map.Add( "Old_Italic", Script.OldItalic );
        //    map.Add( "Javanese", Script.Javanese );
        //    map.Add( "Kayah_Li", Script.KayahLi );
        //    map.Add( "Katakana", Script.Katakana );
        //    map.Add( "Kharoshthi", Script.Kharoshthi );
        //    map.Add( "Khmer", Script.Khmer );
        //    map.Add( "Khojki", Script.Khojki );
        //    map.Add( "Kannada", Script.Kannada );
        //    map.Add( "Kaithi", Script.Kaithi );
        //    map.Add( "Tai_Tham", Script.TaiTham );
        //    map.Add( "Lao", Script.Lao );
        //    map.Add( "Latin", Script.Latin );
        //    map.Add( "Lepcha", Script.Lepcha );
        //    map.Add( "Limbu", Script.Limbu );
        //    map.Add( "Linear_A", Script.LinearA );
        //    map.Add( "Linear_B", Script.LinearB );
        //    map.Add( "Lisu", Script.Lisu );
        //    map.Add( "Lycian", Script.Lycian );
        //    map.Add( "Lydian", Script.Lydian );
        //    map.Add( "Mahajani", Script.Mahajani );
        //    map.Add( "Mandaic", Script.Mandaic );
        //    map.Add( "Manichaean", Script.Manichaean );
        //    map.Add( "Mende_Kikakui", Script.MendeKikakui );
        //    map.Add( "Meroitic_Cursive", Script.MeroiticCursive );
        //    map.Add( "Meroitic_Hieroglyphs", Script.MeroiticHieroglyphs );
        //    map.Add( "Malayalam", Script.Malayalam );
        //    map.Add( "Modi", Script.Modi );
        //    map.Add( "Mongolian", Script.Mongolian );
        //    map.Add( "Mro", Script.Mro );
        //    map.Add( "Meetei_Mayek", Script.MeeteiMayek );
        //    map.Add( "Multani", Script.Multani );
        //    map.Add( "Myanmar", Script.Myanmar );
        //    map.Add( "Old_North_Arabian", Script.OldNorthArabian );
        //    map.Add( "Nabataean", Script.Nabataean );
        //    map.Add( "Nko", Script.Nko );
        //    map.Add( "Ogham", Script.Ogham );
        //    map.Add( "Ol_Chiki", Script.OlChiki );
        //    map.Add( "Old_Turkic", Script.OldTurkic );
        //    map.Add( "Oriya", Script.Oriya );
        //    map.Add( "Osmanya", Script.Osmanya );
        //    map.Add( "Palmyrene", Script.Palmyrene );
        //    map.Add( "Pau_Cin_Hau", Script.PauCinHau );
        //    map.Add( "Old_Permic", Script.OldPermic );
        //    map.Add( "Phags_Pa", Script.PhagsPa );
        //    map.Add( "Inscriptional_Pahlavi", Script.InscriptionalPahlavi );
        //    map.Add( "Psalter_Pahlavi", Script.PsalterPahlavi );
        //    map.Add( "Phoenician", Script.Phoenician );
        //    map.Add( "Miao", Script.Miao );
        //    map.Add( "Inscriptional_Parthian", Script.InscriptionalParthian );
        //    map.Add( "Rejang", Script.Rejang );
        //    map.Add( "Runic", Script.Runic );
        //    map.Add( "Samaritan", Script.Samaritan );
        //    map.Add( "Old_South_Arabian", Script.OldSouthArabian );
        //    map.Add( "Saurashtra", Script.Saurashtra );
        //    map.Add( "SignWriting", Script.SignWriting );
        //    map.Add( "Shavian", Script.Shavian );
        //    map.Add( "Sharada", Script.Sharada );
        //    map.Add( "Siddham", Script.Siddham );
        //    map.Add( "Khudawadi", Script.Khudawadi );
        //    map.Add( "Sinhala", Script.Sinhala );
        //    map.Add( "Sora_Sompeng", Script.SoraSompeng );
        //    map.Add( "Sundanese", Script.Sundanese );
        //    map.Add( "Syloti_Nagri", Script.SylotiNagri );
        //    map.Add( "Syriac", Script.Syriac );
        //    map.Add( "Tagbanwa", Script.Tagbanwa );
        //    map.Add( "Takri", Script.Takri );
        //    map.Add( "Tai_Le", Script.TaiLe );
        //    map.Add( "New_Tai_Lue", Script.NewTaiLue );
        //    map.Add( "Tamil", Script.Tamil );
        //    map.Add( "Tai_Viet", Script.TaiViet );
        //    map.Add( "Telugu", Script.Telugu );
        //    map.Add( "Tifinagh", Script.Tifinagh );
        //    map.Add( "Tagalog", Script.Tagalog );
        //    map.Add( "Thaana", Script.Thaana );
        //    map.Add( "Thai", Script.Thai );
        //    map.Add( "Tibetan", Script.Tibetan );
        //    map.Add( "Tirhuta", Script.Tirhuta );
        //    map.Add( "Ugaritic", Script.Ugaritic );
        //    map.Add( "Vai", Script.Vai );
        //    map.Add( "Warang_Citi", Script.WarangCiti );
        //    map.Add( "Old_Persian", Script.OldPersian );
        //    map.Add( "Cuneiform", Script.Cuneiform );
        //    map.Add( "Yi", Script.Yi );
        //    map.Add( "Inherited", Script.Inherited );
        //    map.Add( "Qaai", Script.Inherited );
        //    map.Add( "Common", Script.Common );
        //    map.Add( "Unknown", Script.Unknown );

        //    return map;
        //}
    }
}
