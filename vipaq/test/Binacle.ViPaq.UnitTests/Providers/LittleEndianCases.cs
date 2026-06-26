namespace Binacle.ViPaq.UnitTests.Providers;

// Shared little-endian vectors: a value paired with the exact bytes it occupies on the wire,
// low byte first. Read tests go bytes -> value; write tests go value -> bytes; same rows both ways.
// Three rows per width: zero, a value whose bytes are all different (pins the order), and all bits.
public static class LittleEndianCases
{
	public static IEnumerable<object[]> UInt16 =>
	[
		[(ushort)0x0000, new byte[] { 0x00, 0x00 }],
		[(ushort)0x0102, new byte[] { 0x02, 0x01 }],
		[(ushort)0xFFFF, new byte[] { 0xFF, 0xFF }],
	];

	public static IEnumerable<object[]> UInt32 =>
	[
		[0x00000000u, new byte[] { 0x00, 0x00, 0x00, 0x00 }],
		[0x01020304u, new byte[] { 0x04, 0x03, 0x02, 0x01 }],
		[0xFFFFFFFFu, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }],
	];

	public static IEnumerable<object[]> UInt64 =>
	[
		[0x0000000000000000UL, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }],
		[0x0102030405060708UL, new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 }],
		[0xFFFFFFFFFFFFFFFFUL, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }],
	];
}
