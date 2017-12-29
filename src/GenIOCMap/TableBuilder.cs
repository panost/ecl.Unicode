using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ecl.Unicode.Ucd;

namespace GenIOCMap {
    class TableBuilder {
        private UcdLoader _loader;

        struct CodeEntry {
            public readonly string Value;
            public readonly int CodePoint;
            public readonly byte Decomposing;

            public CodeEntry( int codePoint, string value, byte decomposing = 0 ) {
                CodePoint = codePoint;
                Value = value;
                Decomposing = decomposing;
            }
        }
        struct BlockEntry {
            public UcdBlock Block;
            public CodeEntry[] Entries;
        }

        private BlockEntry[] _allBlocks;

        public TableBuilder( UcdLoader loader ) {
            _loader = loader;
            var list = new List<CodeEntry>();
            //foreach ( var entry in loader.GetCodePoints() ) {
            //    all[ entry.CodeValue ] = entry.ToString();
            //}
            var blocks = new List<BlockEntry>();
            var decomposing = new List<int>();
            var builder = new StringBuilder();

            string GetDecomposed(int codePoint, bool cascade) {
                decomposing.Clear();
                loader.AddDecomposing( codePoint, decomposing,cascade );
                if ( decomposing.Count <= 1 ) {
                    // accept only multi-code point decombosions
                    return null;
                }
                builder.Clear();
                foreach ( int code in decomposing ) {
                    var entry = loader[ code ];
                    if ( entry.CodeValue == 0 )
                        return null;
                    entry.AppendCharTo( builder );
                }
                return builder.ToString();
            }
            foreach ( UcdBlock block in loader.Blocks ) {
                switch ( block.Block ) {
                case Block.HighSurrogates:
                case Block.HighPrivateUseSurrogates:
                case Block.LowSurrogates:
                case Block.PrivateUseArea:
                case Block.SupplementaryPrivateUseAreaA:
                case Block.SupplementaryPrivateUseAreaB:
                    continue;
                }
                list.Clear();
                foreach ( UnicodeEntry entry in loader.GetCodePoints( block ) ) {
                    list.Add( new CodeEntry(entry.CodeValue, entry.ToString()) );
                    if ( entry.DecomposingLength > 0 ) {
                        string decomp = GetDecomposed( entry.CodeValue, true );
                        if ( decomp != null ) {
                            list.Add( new CodeEntry( entry.CodeValue, decomp, 2 ) );
                            string decomp2 = GetDecomposed( entry.CodeValue, false );
                            if ( decomp2 != null && decomp2 != decomp ) {
                                list.Add( new CodeEntry( entry.CodeValue, decomp2, 1 ) );
                            }
                        }
                    }
                }

                blocks.Add( new BlockEntry() {
                    Block = block,
                    Entries = list.ToArray()
                });
            }

            _allBlocks = blocks.ToArray();
        }

        private readonly string[] _decomposeValues = { "", "*", "**" };
        public void WriteCodes( TextWriter w, StringComparison comparison ) {
            
            for ( var i = 0; i < _allBlocks.Length; i++ ) {
                var leftblock = _allBlocks[ i ];
                Console.WriteLine( "U{0:X6}, {1}", leftblock.Block.Begin, leftblock.Block.Name );
                foreach ( var left in leftblock.Entries ) {
                    if ( left.Decomposing > 0 )
                        continue;
                    for ( int j = 0; j < _allBlocks.Length; j++ ) {
                        var rightblock = _allBlocks[ j ];
                        foreach ( var right in rightblock.Entries ) {
                            if ( right.CodePoint == left.CodePoint && right.Value == left.Value )
                                continue;
                            int cmp = string.Compare( left.Value, right.Value, comparison );
                            if ( cmp == 0 ) {
                                w.WriteLine( "{0:X6} {1:X6} ; ({2}) == ({3}){4}",
                                    left.CodePoint, right.CodePoint, left.Value, right.Value,
                                    _decomposeValues[right.Decomposing]);
                            }
                        }
                    }
                }
            }
        }

        public void WriteCodes( StringComparison comparison ) {
            using ( var w = File.CreateText( @"../../bin/" + comparison + ".txt" ) ) {
                WriteCodes( w, comparison );
            }
        }
    }
}
