using System;
using System.Collections.Generic;
using System.Text;

namespace ecl.Unicode.Cldr;

public class ScriptMetadata {
	public WritingScript Script { get; set; }
	public int Rank { get; internal set; }
	public Territory Territory { get; internal set; }
	public int Density { get; internal set; }
}
