using System.Diagnostics.CodeAnalysis;
using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.ViPaq;

// The harness's only door into ViPaq: the public Serialize / Deserialize, nothing internal (decisions.md
// D4). Every sample uses ushort, so it always goes through the UInt16 path; ViPaq itself decides 8- vs
// 16-bit per section from the values.
//
// Base64 is the real stored form and the headline number, so Encode hands back both the raw bytes (what
// BenchmarkDotNet measures) and the base64 text (what the size report measures).
public static class ViPaqCodec
{
	public static byte[] Encode(PackingSample sample)
		=> ViPaqSerializer.Serialize<Dimensions<ushort>, Item<ushort>, ushort>(sample.Bin, sample.Items);

	public static string ToBase64(byte[] token) 
		=> Convert.ToBase64String(token);

	public static (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Decode(byte[] token)
		=> ViPaqSerializer.Deserialize<Dimensions<ushort>, Item<ushort>, ushort>(token);

	// True only when decode gives back exactly what we serialized. A smaller token that does not round-trip
	// is not a win — the size report rejects it.
	public static bool RoundTrips(PackingSample sample, byte[] token)
	{
		var (bin, items) = Decode(token);

		if (!SameDimensions(sample.Bin, bin))
		{
			return false;
		}

		if (items.Count != sample.Items.Length)
		{
			return false;
		}

		for (var index = 0; index < items.Count; index++)
		{
			var expected = sample.Items[index];
			var actual = items[index];
			var hasSameDimensions = SameDimensions(expected, actual);
			var hasSameCoordinates = SameCoordinates(expected, actual);
			if (!hasSameDimensions || !hasSameCoordinates)
			{
				return false;
			}
		}

		return true;
	}

	private static bool SameDimensions(IWithDimensions<ushort> left, IWithDimensions<ushort> right)
		=> left.Length == right.Length && left.Width == right.Width && left.Height == right.Height;

	private static bool SameCoordinates(IWithCoordinates<ushort> left, IWithCoordinates<ushort> right)
		=> left.X == right.X && left.Y == right.Y && left.Z == right.Z;
}
