namespace Binacle.ViPaq;

// The ViPaq wire format's value limits, straight from the spec (PROTOCOL.md §4-§6). The width ceilings are
// spec numbers, not a runtime type's MaxValue that happens to match — code keys off these so the intent
// ("the 16-bit ceiling") is explicit and the C# and TypeScript implementations stay aligned.
public static class ViPaqLimits
{
	public const ulong EightBitsMax = 255;                 // 2^8  - 1
	public const ulong SixteenBitsMax = 65_535;            // 2^16 - 1
	public const ulong ThirtyTwoBitsMax = 4_294_967_295;   // 2^32 - 1

	// The interoperable ceiling: the largest integer every target runtime holds exactly (2^53 - 1,
	// JavaScript's Number.MAX_SAFE_INTEGER). The 64-bit width can hold more, but ViPaq forbids it. PROTOCOL.md §5.
	public const ulong MaxInteger = 9_007_199_254_740_991; // 2^53 - 1

	// Compression trigger (PROTOCOL.md §6): gzip the body when its length is greater than this many bytes.
	// Shares the number 255 with EightBitsMax by coincidence, not meaning — keep them separate.
	public const int CompressionThresholdBytes = 255;
}
