import {BitSize} from "../models";

export class Sizes {
	public static bitSizes: Record<string, BitSize> = {
		// For byte, sbyte, short, ushort, int, uint, long, ulong, use 'number'
		'byte': BitSize.Eight,
		'sbyte': BitSize.Eight,
		'short': BitSize.Sixteen,
		'ushort': BitSize.Sixteen,
		'int': BitSize.ThirtyTwo,
		'uint': BitSize.ThirtyTwo,
		'long': BitSize.SixtyFour,
		'ulong': BitSize.SixtyFour,
	};

	// Width ceilings straight from the spec (PROTOCOL.md §4), not a runtime type's max that happens to
	// match. Code keys off these so "the 16-bit ceiling" is explicit and both implementations stay aligned.
	public static eightBitsMax = 255;               // 2^8  - 1
	public static sixteenBitsMax = 65_535;          // 2^16 - 1
	public static thirtyTwoBitsMax = 4_294_967_295; // 2^32 - 1

	// ViPaq's interoperable integer ceiling: the largest integer every target runtime holds exactly
	// (2^53 - 1, Number.MAX_SAFE_INTEGER). The 64-bit width can hold more, but values above this are
	// outside the protocol — see PROTOCOL.md §5. All dimensions and coordinates must be in [0, maxInteger].
	public static maxInteger = 9_007_199_254_740_991; // 2^53 - 1

	// Compression trigger (PROTOCOL.md §6): gzip the body when its length is greater than this many bytes.
	// Shares the number 255 with eightBitsMax by coincidence, not meaning — keep them separate.
	public static compressionThresholdBytes = 255;
}
