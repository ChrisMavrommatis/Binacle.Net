using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Google.Protobuf;

namespace Binacle.ViPaq.TestsKernel.Protobuf;

// Turns a scenario into protobuf bytes — the baseline ViPaq is measured against. To keep the comparison fair it
// runs the *same codec* as the ViPaq side of the same file: the NoOp file compares raw against
// raw, the deflate file deflate against deflate, the gzip file gzip against gzip. So the only thing that differs
// in a table is the format, never the compressor. Protobuf has no layout, so the two layout tables in a file
// share the same protobuf bytes.
//
// It compresses through the library's own `DeflateCodec` / `GzipCodec`, not a hand-rolled stream, or the two
// sides would silently run different compressor settings.
public class ProtobufEncoder
{
	private readonly ICompressionCodec compressionCodec;

	public ProtobufEncoder(ICompressionCodec compressionCodec)
	{
		this.compressionCodec = compressionCodec;
	}

	// Protobuf has no layout and the race always compresses, so there is nothing to vary per call — it just runs
	// the file's codec. In the NoOp file that codec passes the bytes through, which is the raw protobuf baseline.
	public byte[] Encode(Scenario scenario)
	{
		var raw = ToMessage(scenario).ToByteArray();
		return this.compressionCodec.Compress(raw);
	}

	// The mirror of Encode, for the decode baseline: inflate with the same codec, then parse. Like-for-like with
	// ViPaq's decode, which also decompresses before it reads. The bytes must have come from this codec's Encode.
	public PackedResult Decode(byte[] bytes)
	{
		var raw = this.compressionCodec.Decompress(bytes);
		return PackedResult.Parser.ParseFrom(raw);
	}

	private static PackedResult ToMessage(Scenario scenario)
	{
		var message = new PackedResult
		{
			Count = (uint)scenario.Items.Length,
			Bin = new Vec3
			{
				Length = scenario.Bin.Length,
				Width = scenario.Bin.Width,
				Height = scenario.Bin.Height
			}
		};

		foreach (var item in scenario.Items)
		{
			message.Items.Add(new PlacedItem
			{
				Length = item.Length,
				Width = item.Width,
				Height = item.Height,
				X = item.X,
				Y = item.Y,
				Z = item.Z
			});
		}

		return message;
	}
}
