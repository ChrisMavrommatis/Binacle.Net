using System.Numerics;
using Binacle.ViPaq.Compression;

namespace Binacle.ViPaq.UnitTests;

// The lower path: encode and decode through ProtocolEncoder, which is handed a header rather than choosing
// one. That is the whole point of driving it directly — layout and all three widths are inputs, so a test can
// force a columnar or wider blob. ViPaqSerializer chooses nothing yet (it always writes raw, row-major,
// narrowest), so a test driven through it can never reach the layout or width variants. For the path a real
// caller takes, use ViPaqSerializerTestingFixture instead.
//
// These helpers are uncompressed: the encoder gets the NoOp codec, so the body stays byte-for-byte readable
// (the exact-byte pins depend on that). The exception is DecodeWith, which takes a codec so the
// cross-language decode test can read deflate/gzip blobs (raw passes a NoOp).
//
// Nothing here checks anything. Every method encodes or decodes and hands back a BinContents, so a test
// keeps its arrange, act and assert on three separate lines and compares with BinContents.AssertSame.
internal static class ProtocolTestingFixture
{
	// Builds a whole blob under a caller-chosen header. The header decides widths and layout; ProtocolEncoder
	// obeys it. Uncompressed only, so the codec is always NoOp.
	public static byte[] Encode<T>(
		Header header,
		Binacle.Geometry.Dimensions<T> bin,
		IReadOnlyList<Binacle.Geometry.Item<T>> items
	)
		where T : struct, IBinaryInteger<T>
	{
		var encoder = new ProtocolEncoder(new NoOpCodec());

		return encoder.Encode<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(header, bin, items);
	}

	// Round-trips under a caller-chosen header, so the test controls compression, layout and widths.
	public static BinContents<T> RoundTrip<T>(Header header, BinContents<T> binContents)
		where T : struct, IBinaryInteger<T>
	{
		var encoded = Encode(header, binContents.Bin, binContents.Items);

		return Deserialize<T>(encoded);
	}

	// Convenience for the width matrix, which only needs some valid header to round-trip: uses the library's
	// default (raw, row-major, narrowest) — the same header ViPaqSerializer would pick, but reached through
	// ProtocolEncoder like every other path here.
	public static BinContents<T> RoundTrip<T>(BinContents<T> binContents)
		where T : struct, IBinaryInteger<T>
	{
		var header = Header.Create<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(binContents.Bin, binContents.Items);

		return RoundTrip(header, binContents);
	}

	// Decode a whole blob (the two header bytes plus the body). The header is read off the wire; every blob
	// on this path is uncompressed, so the codec is NoOp. Pins decode against literal bytes, not the encoder.
	public static BinContents<T> Deserialize<T>(byte[] data)
		where T : struct, IBinaryInteger<T>
	{
		var codec = new NoOpCodec();

		return DecodeWith<T>(data, codec);
	}

	// Decodes through ProtocolEncoder + the given codec — the cross-language decode test passes the codec
	// named by the artifact file (raw = NoOp, deflate/gzip = the real codec), so one call decodes every mode.
	public static BinContents<T> DecodeWith<T>(byte[] data, ICompressionCodec codec)
		where T : struct, IBinaryInteger<T>
	{
		var header = Header.FromBytes(data[0], data[1]);
		var body = data[Header.ByteCount..];
		var encoder = new ProtocolEncoder(codec);

		var (resultBin, resultItems) = encoder.Decode<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(header, body);

		return new BinContents<T>(resultBin, resultItems.AsReadOnly());
	}
}
