namespace Binacle.ViPaq;

// The ViPaq wire format's value limits, straight from the spec (PROTOCOL.md §5). Code keys off these so the
// intent ("the 16-bit ceiling") is explicit and the C# and TypeScript implementations stay aligned.
public static class Limits
{
	// The largest value an Eight-width field holds (2^8 - 1).
	public const int EightBitsMax = 255;

	// Every dimension and coordinate is in [0, MaxValue] (2^16 - 1). There is no wider width, no saturation,
	// and no widening — encoding a value above this is an error.
	public const int MaxValue = 65_535;

	// The item count is a uint16, so it shares the ceiling but not the meaning.
	public const int MaxItemCount = 65_535;
}
