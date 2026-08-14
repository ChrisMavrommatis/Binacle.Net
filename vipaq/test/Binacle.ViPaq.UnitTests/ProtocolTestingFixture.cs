using System.Numerics;
using Binacle.ViPaq.Compression;

namespace Binacle.ViPaq.UnitTests;

// The lower path: encode and decode through ProtocolEncoder, which is handed a header rather than choosing one.
// Layout and all three widths are inputs, so a test can force a columnar or deliberately-too-wide blob -
// unreachable through ViPaqSerializer, which derives the widths itself. Use ViPaqSerializerTestingFixture for
// the path a real caller takes.
//
// These helpers are uncompressed: the encoder gets the NoOp codec, so the body stays byte-for-byte readable and
// the exact-byte pins hold. DecodeWith is the exception, taking a codec so the cross-language decode test can
// read deflate/gzip blobs.
//
// Nothing here checks anything. Every method hands back a BinContents to compare with BinContents.AssertSame.
internal static class ProtocolTestingFixture
{
	// A whole blob under a caller-chosen header. Uncompressed only, so the codec is always NoOp.
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

	// For the width matrix, which only needs some valid header: the library's default (raw, row-major,
	// narrowest), reached through ProtocolEncoder like every other path here.
	public static BinContents<T> RoundTrip<T>(BinContents<T> binContents)
		where T : struct, IBinaryInteger<T>
	{
		var header = Header.Create<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(binContents.Bin, binContents.Items);

		return RoundTrip(header, binContents);
	}

	// Decode a whole blob, header read off the wire. Uncompressed, so the codec is NoOp. Pins decode against
	// literal bytes, not the encoder.
	public static BinContents<T> Deserialize<T>(byte[] data)
		where T : struct, IBinaryInteger<T>
	{
		var codec = new NoOpCodec();

		return DecodeWith<T>(data, codec);
	}

	// The cross-language decode test passes the codec named by the artifact file, so one call decodes every
	// mode.
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
