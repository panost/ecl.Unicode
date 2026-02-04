# Unicode and CLDR readers (ecl.Unicode)

The main purpose of these readers is to facilitate the generation of code or tables using the files published by the Unicode Consortium, specifically for the Unicode Character Database (UCD) and the Common Locale Data Repository (CLDR). The readers are designed to consume both data extracted in a specified folder or directly from the .zip file as it is downloaded from the Unicode site.

Using the compressed form, this library can outperform Java or JavaScript code generation by two orders of magnitude.

## Data sources

UCD: https://www.unicode.org/Public/zipped/15.1.0/UCD.zip
CLDR: https://unicode.org/Public/cldr/44/cldr-common-44.0.zip

## What this library provides

- Strongly-typed models for UCD code points, blocks, properties, scripts, and break/emoji ranges.
- CLDR locale documents with fallback (root -> parent -> locale) plus supplemental data for calendars, territories, time zones, currencies, and units.
- A unified file loader that reads from either zip archives or extracted directories.
- Fast, allocation-light parsing with lazy UCD data loading.

## Project layout

- `Ucd/` contains UCD domain models, range utilities, and text-file parsing logic.
- `Cldr/Doc/` contains the CLDR XML loader and document infrastructure.
- `Cldr/Locale/` contains locale-level objects after parsing.
- Root-level helpers such as `ZipLoader`, `XUtil`, `WritingScript`, and `NumberPattern` provide shared parsing and metadata utilities.

## Usage

### Read CLDR currencies

```csharp
using System;
using ecl.Unicode.Cldr.Doc;

// Read CLDR currencies
using (var cldr = new CldrLoader(@"C:\Cldr\44\cldr-common-44.0.zip")) {
    var en = cldr.GetLocale("en-US");
    foreach (var currency in cldr.GetCurrencies()) {
        Console.WriteLine($"Code:{currency.Code} {currency.GetName(en)} " +
            $"Fractional = {currency.CashDigits} UnitsPerBase = {(int)Math.Pow(10, currency.Digits)}");
    }
}
```

### Read UCD code points and decompositions

```csharp
using System;
using System.Collections.Generic;
using ecl.Unicode.Ucd;

// Get the decomposed maps of a codepoint range
using (var ucd = new UcdLoader(@"C:\Ucd\UCD.zip", UcdLoader.LoadOptions.AllCodes)) {
    List<int> list = new List<int>();

    foreach (UnicodeEntry pt in ucd.GetCodePoints(0x80, 0x052F)) {
        if (pt.Category <= UnicodeCharacterType.LetterOther) {
            if (pt.Decomposing == 0 && ucd.AddDecomposing(pt.CodeValue, list, true)) {
                foreach (int i in list) {
                    if (ucd.TryGetEntry(i, out UnicodeEntry other)
                        && other.Category <= UnicodeCharacterType.LetterOther) {
                        Console.WriteLine($"Code:{i}, upper:{pt.ToUpper()} / {other.ToUpper()}");
                    }
                }
            }
        }
    }
}
```

### More UCD examples

```csharp
using System;
using System.Linq;
using ecl.Unicode.Ucd;

using (var ucd = new UcdLoader(@"C:\Ucd\UCD.zip", UcdLoader.LoadOptions.AllCodes)) {
    // Find the block for a code point
    var block = ucd.GetBlock(0x1F600);
    Console.WriteLine(block?.Name);

    // Query code point properties
    var props = ucd.GetCodeProperties(0x0041); // 'A'
    if (props != null) {
        Console.WriteLine(string.Join(", ", props.Select(p => p.Name)));
    }

    // Load word break ranges
    var wordBreak = ucd.LoadWordBreak();
    Console.WriteLine($"WordBreak ranges: {wordBreak.Length}");
}
```

### More CLDR examples

```csharp
using System;
using ecl.Unicode.Cldr.Doc;

using (var cldr = new CldrLoader(@"C:\Cldr\44\cldr-common-44.0.zip")) {
    // Calendar preference for a territory
    var calendars = cldr.GetCalendarPreference("US");
    Console.WriteLine($"Default calendar: {calendars?[0]?.Name}");

    // Resolve a territory and its description (after EnsureEnglishNames)
    cldr.EnsureEnglishNames();
    var territory = cldr.FindTerritory("US");
    Console.WriteLine(territory?.Description);
}
```

## Build and pack

From the repository root:

```powershell
# build
cd .\src\ecl.Unicode
 dotnet build -c Release

# create a NuGet package
 dotnet pack -c Release -o .\artifacts
```

## Target framework

- netstandard2.0

## Notes

- Both loaders implement `IDisposable`; dispose them to release file handles for zip archives.
- UCD data is loaded lazily; the first access to code points triggers parsing.
- CLDR locale resolution falls back from a requested locale to its parent and to `root` when needed.
