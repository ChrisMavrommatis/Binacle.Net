// Ports C#: Limits. Spec ceilings (PROTOCOL.md §4/§5), not a runtime type's max that happens to match.
export class Sizes {
	public static eightBitsMax = 255;    // 2^8  - 1, the widest an 8-bit section holds
	public static sixteenBitsMax = 65_535; // 2^16 - 1, the widest a 16-bit section holds

	// ViPaq's interoperable ceiling: every value fits in 16 bits. Above this is outside the protocol and is
	// rejected (PROTOCOL.md §5).
	public static maxValue = 65_535;

	// The item count is a uint16, so a blob carries at most this many items (PROTOCOL.md §3).
	public static maxItemCount = 65_535;
}
