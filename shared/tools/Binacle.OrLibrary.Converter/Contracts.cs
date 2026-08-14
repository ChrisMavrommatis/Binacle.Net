namespace Binacle.OrLibrary.Converter;

// The tests-kernel compact scenario, in the exact shape and property order the kernel reads and writes. One
// array per thpack file.
//
//   Name    — "OrLibrary_thpack{file}_{problem index}", e.g. "OrLibrary_thpack1_1".
//   Bin      — the container as "LxWxH".
//   Metrics  — "ItemsVolume BinVolume ItemsCount Percentage": totals over all box types, and their volume ratio.
//   Result   — "{PackingStatus} {FittingStatus}": the expected outcome the tests assert against.
//   Items    — the box types as "LxWxH [Quantity]" (types with a count, not placed items — no coordinates).
internal sealed class Scenario
{
	public required string Name { get; init; }
	public required string Bin { get; init; }
	public required string Metrics { get; init; }
	public required string Result { get; init; }
	public required string[] Items { get; init; }
}
