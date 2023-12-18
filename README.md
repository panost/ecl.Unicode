# Unicode and CLDR readers

The main purpose of those readers is facilitate the generation of code or tables using the files published by the [Unicode Consortium](https://home.unicode.org/), specifically for the [Unicode Character Database](https://www.unicode.org/reports/tr44/tr44-32.html) (UCD) and the [Common Locale Data Repository](https://cldr.unicode.org/index) (CLDR).
The readers designed to consume both data extracted in a specified folder or directly from the .zip file as it is downloaded from the Unicode site. Specifically, the following files. 

UCD: https://www.unicode.org/Public/zipped/15.1.0/UCD.zip
CLDR: https://unicode.org/Public/cldr/44/cldr-common-44.0.zip

Using the compressed form, this library outperforms Java or Javascript code generation by two orders of magnitude.

## Usage


```csharp
// Read CLDR Currencies

using ( var cldr = new CldrLoader( @"C:\Cldr\44\cldr-common-44.0.zip" ) ) {
	var _en = cldr.GetLocale( "en-US" );
	foreach ( Currency currency in cldr.GetCurrencies() ) {
		Console.WriteLine( $"Code:{currency.Code} {currency.GetName( _en )} " +
			$"Fractional = {currency.CashDigits} UnitsPerBase = {(int)Math.Pow( 10, currency.Digits )}" );
	}
}
```

using UCD

```csharp
// Get the decomposed maps of a codepoint range
using (var ucd = new UcdLoader( @"f:\Resource\Unicode\8.0.0\UCD.zip", UcdLoader.LoadOptions.AllCodes ) ) {
	List<int> list = new List<int>();

	foreach ( UnicodeEntry pt in ucd.GetCodePoints( 0x80, 0x052F ) ) {
		if ( pt.Category <= UnicodeCharacterType.LetterOther ) {
			if ( pt.Decomposing == 0 && ucd.AddDecomposing( pt.CodeValue, list, true ) ) {
				int code = pt.CodeValue;

				foreach ( int i in list ) {
					UnicodeEntry other;
					if ( ucd.TryGetEntry( i, out other )
							&& other.Category <= UnicodeCharacterType.LetterOther ) {
						Console.WriteLine( $"Code:{i}, upper:{pt.ToUpper()} / {other.ToUpper()}" );
					}
				}
			}
		}
	}
}
```
