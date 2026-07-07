namespace Binacle.ViPaq.TestsKernel.Models;
// [REVIEW-VIPAQ_TEST]
// One benchmark input: a bin plus placed items, exactly what both ViPaq and protobuf serialize.
// Uses the shared Binacle.Geometry types so ViPaq's public Serialize takes it as-is and no second
// model can drift from it.
//
// WidthBits and Spread are what we *asked the generator for*. They label the sample so reports can
// group by shape. What ViPaq actually stored (8- or 16-bit per section, compressed or not) is read
// back from the token's header — see ViPaqHeader. Never trust these tags as ground truth for size.
public sealed record PackingSample
{
	public required string Name { get; init; }
	public required Dimensions<ushort> Bin { get; init; }
	public required Item<ushort>[] Items { get; init; }

	// The width family we aimed for: 8 (all values fit in a byte) or 16 (something needs two bytes).
	public required int WidthBits { get; init; }

	// How item values are spread: "low" near 0, "high" near the width max, "mixed" across the range.
	public required string Spread { get; init; }

	// For real samples only: the ViPaq token the source (the API) emitted for this exact data. Kept as a
	// cross-check — we re-encode here and compare — never as the measured size. Null for synthetic samples.
	public string? SourceToken { get; init; }

	public int ItemCount => this.Items.Length;
}
