namespace Binacle.ViPaq.TestsKernel.Models;

// One benchmark input: a bin plus placed items, exactly what both ViPaq and protobuf serialize. Uses the shared
// Binacle.Geometry types so no second model can drift from them.
//
// WidthBits and Spread are what the generator was asked for, and only label the scenario. Never trust them as
// ground truth for size - what ViPaq actually stored is read back from the token's header.
public sealed record Scenario
{
	public required string Name { get; init; }
	public required Dimensions<ushort> Bin { get; init; }
	public required Item<ushort>[] Items { get; init; }

	// The width family we aimed for: 8 (all values fit in a byte) or 16 (something needs two bytes).
	public required int WidthBits { get; init; }

	// How item values are spread: "low" near 0, "high" near the width max, "mixed" across the range.
	public required string Spread { get; init; }

	public int ItemCount => this.Items.Length;
}
