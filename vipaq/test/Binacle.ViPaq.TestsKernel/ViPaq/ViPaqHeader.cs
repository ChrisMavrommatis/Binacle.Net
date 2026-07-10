using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.ViPaq;

// What the harness needs to know about a token, read off its header.
//
// It answers two questions: "did ViPaq compress?" (so the harness can mirror that on protobuf) and "which width
// did ViPaq pick?" (so a report can confirm the scenario landed where we expected). It also gives the size the
// token would have had raw, which the crossover report sets the real token against.
//
// It re-reads nothing. The library's own `Header` does the parsing and the size arithmetic, and this type just
// hands the answers out in terms the reports can print — `Header`, `Width` and `Layout` are all internal, so
// they cannot appear on a public member here. Keeping the spec in one place is why the library grants
// `InternalsVisibleTo` to this project. `Compressed` is its own header bit; it is not a `Version`, as it was
// before the rewrite.
public readonly record struct ViPaqHeader
{
	internal readonly Header Header;

	private ViPaqHeader(Header header)
	{
		this.Header = header;
	}

	// Throws ViPaqFormatException on a malformed header — an unsupported version, a set reserved bit, or a
	// reserved width code.
	public static ViPaqHeader Read(byte[] token)
		=> new(Header.FromBytes(token[0], token[1]));

	// Builds the header for a scenario in a forced mode, before any token exists. The widths come from the
	// library's own width choice (`Header.Create`, the one spec that decides them), then the mode is stamped on:
	// the race always compresses (NoOp is how it prices the raw size), so `Compressed` is always set, and `Layout`
	// comes from the mode. This is what the encoder is handed to force a mode.
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
