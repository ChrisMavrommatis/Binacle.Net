using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.ViPaq;

// What the harness needs to know about a token, read off its header: did ViPaq compress (so the harness can
// mirror that on protobuf), which width did it pick, and what would the token have cost raw.
//
// The library's own `Header` does the parsing and the size arithmetic; this only prints the answers. `Header`,
// `Width` and `Layout` are internal, so they cannot appear on a public member here - which is why the library
// grants `InternalsVisibleTo` to this project.
public readonly record struct ViPaqHeader
{
	internal readonly Header Header;

	private ViPaqHeader(Header header)
	{
		this.Header = header;
	}

	// Throws ViPaqFormatException on a malformed header.
	public static ViPaqHeader Read(byte[] token)
		=> new(Header.FromBytes(token[0], token[1]));

	// The header for a scenario in a forced mode, before any token exists. Widths come from `Header.Create`,
	// then the mode is stamped on. The race always compresses - NoOp is how it prices the raw size - so
	// `Compressed` is always set.
	public static ViPaqHeader Create(Scenario scenario, EncoderInfo encoderInfo)
	{
		var header = Header.Create<Dimensions<ushort>, Item<ushort>, ushort>(
			scenario.Bin,
			scenario.Items
		);

		var modifiedHeader = header
			with
			{
				Compressed = true,
				Layout = encoderInfo.Layout
			};

		return new ViPaqHeader(modifiedHeader);
	}

	public bool IsCompressed 
		=> this.Header.Compressed;

	public int BinDimensionsBits 
		=> Bits(this.Header.BinDimensionsWidth);

	public int ItemDimensionsBits 
		=> Bits(this.Header.ItemDimensionsWidth);

	public int ItemCoordinatesBits 
		=> Bits(this.Header.ItemCoordinatesWidth);

	// Exact, not an estimate: the two header bytes plus a body of fixed-width fields.
	public int UncompressedByteCount(int itemCount)
		=> Header.ByteCount + this.Header.GetBodyLength(itemCount);

	// How a report names the three widths, in wire order.
	public string ToWidthsLabel()
		=> $"{this.BinDimensionsBits}/{this.ItemDimensionsBits}/{this.ItemCoordinatesBits}";

	private static int Bits(Width width) 
		=> Helpers.WidthHelper.ByteCount(width) * 8;
}
