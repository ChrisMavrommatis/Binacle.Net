using Binacle.CompactNotation;
using Binacle.ViPaq.TestsKernel.Models;
// [REVIEW-VIPAQ_TEST]
namespace Binacle.ViPaq.TestsKernel.Samples;

// Real packed results for the custom and Bischoff (thpack1..7) problems: each sample is the bin plus the
// *placed* items the packer produced (L,W,H and X,Y,Z), with Spread = "real".
//
// The data is generated offline by Binacle.ViPaq.PackedDataGenerator (FFD, pinned) and committed under
// vipaq/data/packed/{bischoff-suite,custom-problems}/; this provider reads those files as embedded resources at
// static init. It is no longer the old API-captured hardcode. Only placed geometry is stored (no token) — the
// token is derivable and its compressed bytes vary by runtime, so we compute it here when we benchmark.
// Regenerate the data with the tool; do not hand-edit.
public static class RealDataProvider
{
	private const string BischoffFamily = "bischoff-suite";

	private static readonly Dictionary<string, PackingSample> samples = new();
	private static readonly List<PackingSample> bischoff = [];
	private static readonly List<PackingSample> custom = [];

	static RealDataProvider()
	{
		foreach (var (family, record) in PackedDataReader.ReadAll())
		{
			var sample = ToSample(record);
			samples.Add(sample.Name, sample);
			(family == BischoffFamily ? bischoff : custom).Add(sample);
		}
	}

	private static PackingSample ToSample(PackedDataReader.PackedSampleRecord record)
	{
		// Parse as int (the notation's natural width) then narrow to ushort with a checked cast. Placed values
		// fit comfortably — the largest Bischoff coordinate is ~587, far below ushort's 65535 — so the cast is
		// a guard, not a lossy conversion.
		var binInt = CompactNotationParser.ParseDimensions<int>(record.Bin);
		var bin = new Dimensions<ushort>
		{
			Length = checked((ushort)binInt.Length),
			Width = checked((ushort)binInt.Width),
			Height = checked((ushort)binInt.Height),
		};

		var items = new Item<ushort>[record.Items.Length];
		for (var index = 0; index < items.Length; index++)
		{
			var item = CompactNotationParser.ParseItem<int>(record.Items[index]);
			items[index] = new Item<ushort>
			{
				Length = checked((ushort)item.Length),
				Width = checked((ushort)item.Width),
				Height = checked((ushort)item.Height),
				X = checked((ushort)item.X),
				Y = checked((ushort)item.Y),
				Z = checked((ushort)item.Z),
			};
		}

		return new PackingSample
		{
			Name = record.Name,
			WidthBits = record.WidthBits,
			Spread = "real",
			Bin = bin,
			Items = items,
		};
	}

	public static IReadOnlyCollection<PackingSample> All => samples.Values;

	// The two source families, split out so reports can show them as separate tables.
	public static IReadOnlyCollection<PackingSample> Bischoff => bischoff;

	public static IReadOnlyCollection<PackingSample> Custom => custom;

	public static IEnumerable<string> Names => samples.Keys;

	public static PackingSample GetByName(string name) => samples[name];

	// A representative slice for BenchmarkDotNet: a dense custom pack and two Bischoff sizes. These names still
	// resolve in the generated data, so the slice is unchanged from the API-captured version.
	public static IEnumerable<string> BenchmarkNames =>
	[
		"Simple_5x5x5-100_FitIn_60x40x10",
		"OrLibrary_thpack4_1",
		"OrLibrary_thpack1_2"
	];
}
