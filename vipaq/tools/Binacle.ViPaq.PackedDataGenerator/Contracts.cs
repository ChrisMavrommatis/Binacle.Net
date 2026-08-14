namespace Binacle.ViPaq.PackedDataGenerator;

// A source family: its input directory, the destination subfolder for its placed results, and its problem
// files. All directory knowledge lives in the caller; the generator does no ad-hoc path building.
internal sealed record SourceFamily(string[] InputDir, string DestinationFolder, string[] Files);

// What one family's run produced.
internal sealed record GenerationResult(int Samples, int Items);

// One row of a source problem file. Only Name/Bin/Items are read; Metrics and Result are fields the packer
// does not need. Items are box types with a count ("LxWxH [Q]") - coordinates only exist after packing.
internal sealed class SourceScenario
{
	public required string Name { get; init; }
	public required string Bin { get; init; }
	public required string[] Items { get; init; }
}

// One placed-result row this tool writes. PascalCase keys and compact-notation strings match the test-vectors
// conventions. WidthBits is 8 if every bin dim, item dim and coord fits in a byte, else 16. No token is stored
// - it is derivable from the geometry, and its compressed bytes vary by runtime.
internal sealed class PackedSample
{
	public required string Name { get; init; }
	public required int WidthBits { get; init; }
	public required string Bin { get; init; }
	public required string[] Items { get; init; }
}
