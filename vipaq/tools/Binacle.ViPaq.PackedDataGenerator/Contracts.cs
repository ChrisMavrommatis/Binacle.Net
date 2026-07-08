namespace Binacle.ViPaq.PackedDataGenerator;

// One row of a source problem file (shared/data/bischoff-suite or custom-problems). Only Name/Bin/Items are
// read: Metrics and Result are arithmetic / expected-status fields the packer doesn't need. Items are box
// *types* with a count ("LxWxH [Q]"), no coordinates — the coordinates only exist after packing.
internal sealed class SourceScenario
{
	public required string Name { get; init; }
	public required string Bin { get; init; }
	public required string[] Items { get; init; }
}

// One placed-result row this tool writes, per problem. PascalCase keys and compact-notation strings match the
// test-vectors conventions. WidthBits is derived (8 if every bin dim, item dim and coord fits in a byte, else
// 16). Bin is "LxWxH"; Items are placed items "LxWxH (X,Y,Z)". No token is stored — it is derivable from the
// geometry and its compressed bytes vary by runtime, so the kernel computes it when it benchmarks.
internal sealed class PackedSample
{
	public required string Name { get; init; }
	public required int WidthBits { get; init; }
	public required string Bin { get; init; }
	public required string[] Items { get; init; }
}
