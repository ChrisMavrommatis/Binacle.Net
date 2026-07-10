namespace Binacle.ViPaq.PackedDataGenerator;

// A source family: its input directory (segments under the repo root), the destination subfolder for its placed
// results, and the problem files it contains. All directory knowledge lives in the caller — the generator
// resolves these against the repo root and does no ad-hoc path building.
internal sealed record SourceFamily(string[] InputDir, string DestinationFolder, string[] Files);

// What one family's run produced.
internal sealed record GenerationResult(int Samples, int Items);

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
