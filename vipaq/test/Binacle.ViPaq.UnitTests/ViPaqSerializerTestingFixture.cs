using System.Numerics;

namespace Binacle.ViPaq.UnitTests;

// The public path: everything goes through ViPaqSerializer, the entry point a real caller uses. It picks its
// own header — raw, row-major, narrowest — so nothing here takes one. Choosing that header is ViPaqSerializer's
// own job (PROTOCOL.md §4/§6), which is exactly what these tests pin.
//
// Use ProtocolTestingFixture instead when a test needs to force a header: a compressed, columnar or
// deliberately wide blob is unreachable from this side.
//
// Nothing here checks anything. Every method serializes or deserializes and hands back a BinContents, so a
// test keeps its arrange, act and assert on three separate lines and compares with BinContents.AssertSame.
internal static class ViPaqSerializerTestingFixture
{
	// Serialize through the public API. `configure` is the opt-in options hook (compression, layout); leaving
	// it null is the default a plain caller gets.
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
