namespace Binacle.ViPaq.TestsKernel.ViPaq;

// Reads ViPaq's one-byte header (byte 0) using only the public Version and BitSize enums.
//
// The permanent harness stays on ViPaq's public surface (see decisions.md D4), so it must not call the
// internal header helper. Instead it re-reads the documented header layout (PROTOCOL.md): two bits each
// for version, bin-dimension width, item-dimension width, item-coordinate width, high bits first.
//
// This is how the harness answers two questions without touching internals: "did ViPaq compress?" (so it
// can mirror that on protobuf) and "which width did ViPaq pick?" (so a report can confirm the sample
// landed where we expected).
public readonly record struct ViPaqHeader(
	Binacle.ViPaq.Version Version,
	BitSize BinDimensionsBitSize,
	BitSize ItemDimensionsBitSize,
	BitSize ItemCoordinatesBitSize
)
{
	public bool IsCompressed => this.Version == Binacle.ViPaq.Version.CompressedGzip;

	public static ViPaqHeader Read(byte[] token)
	{
		var header = token[0];
		return new ViPaqHeader(
			(Binacle.ViPaq.Version)((header & 0b1100_0000) >> 6),
			(BitSize)((header & 0b0011_0000) >> 4),
			(BitSize)((header & 0b0000_1100) >> 2),
			(BitSize)(header & 0b0000_0011)
		);
	}
}
