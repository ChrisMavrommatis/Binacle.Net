using System.Numerics;

namespace Binacle.ViPaq.UnitTests;

// The public path: everything goes through ViPaqSerializer, the entry point a real caller uses. It derives the
// widths itself and takes compression and layout from the caller's options. Pinning that choice (PROTOCOL.md
// §4/§6) is what these tests are for.
//
// Use ProtocolTestingFixture when a test needs to force the widths - a deliberately wide blob is unreachable
// from this side.
//
// Nothing here checks anything. Every method hands back a BinContents to compare with BinContents.AssertSame.
internal static class ViPaqSerializerTestingFixture
{
	// `configure` is the opt-in options hook; leaving it null is what a plain caller gets.
	public static byte[] Serialize<T>(
		BinContents<T> binContents,
		Action<ViPaqSerializationOptions>? configure = null
	)
		where T : struct, IBinaryInteger<T>
		=> ViPaqSerializer.Serialize<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(binContents.Bin, binContents.Items, configure);

	// Deserialize a blob the public API produced.
	public static BinContents<T> Deserialize<T>(byte[] data)
		where T : struct, IBinaryInteger<T>
	{
		var (resultBin, resultItems) = ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<T>, Binacle.Geometry.Item<T>, T>(data);

		return new BinContents<T>(resultBin, resultItems.AsReadOnly());
	}

	// Serialize then Deserialize, the round trip a real caller actually performs.
	public static BinContents<T> RoundTrip<T>(BinContents<T> binContents)
		where T : struct, IBinaryInteger<T>
	{
		var data = Serialize(binContents);

		return Deserialize<T>(data);
	}
}
