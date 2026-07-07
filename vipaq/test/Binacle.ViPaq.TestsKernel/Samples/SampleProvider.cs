using Binacle.ViPaq.TestsKernel.Models;
// [REVIEW-VIPAQ_TEST]
namespace Binacle.ViPaq.TestsKernel.Samples;

// The one catalog of benchmark inputs. Both consumers read from here, so both measure the same data:
//  - the size / round-trip runner walks the full catalog (it is cheap),
//  - BenchmarkDotNet walks BenchmarkNames only (a full BDN run per sample is not).
//
// The matrix: width {8, 16} x spread {low, high, mixed} x item count {5, 13, 50, 2000, 5000}, plus two
// hand-made samples at the 255 -> 256 boundary. Item counts climb high on purpose: past ~255 body bytes
// ViPaq starts compressing, which is where the crossover report finds its answer.
public static class SampleProvider
{
	private static readonly int[] itemCounts = [5, 13, 50, 2_000, 5_000];
	private static readonly int[] widths = [8, 16];
	private static readonly string[] spreads = ["low", "high", "mixed"];

	private static readonly Dictionary<string, PackingSample> samples;

	// A static constructor makes it explicit that the whole catalog is built once, on first access.
	static SampleProvider()
	{
		samples = new Dictionary<string, PackingSample>();

		// A stable, non-repeating seed per sample so runs match but samples differ from each other.
		var seed = 1;
		foreach (var width in widths)
		{
			foreach (var spread in spreads)
			{
				foreach (var count in itemCounts)
				{
					var name = $"{width}bit-{spread}-{count}";
					samples.Add(name, SampleGenerator.Generate(name, width, spread, count, seed++));
				}
			}
		}

		foreach (var boundarySample in BoundarySamples())
		{
			samples.Add(boundarySample.Name, boundarySample);
		}
	}

	public static IReadOnlyCollection<PackingSample> All => samples.Values;

	// Names for the size / round-trip runner (the full catalog).
	public static IEnumerable<string> Names => samples.Keys;

	public static PackingSample GetByName(string name) => samples[name];

	// A small, representative slice for BenchmarkDotNet: both widths, a low / mid / high item count,
	// mixed spread. Enough to see encode/decode cost scale without a benchmark run per catalog entry.
	public static IEnumerable<string> BenchmarkNames =>
	[
		"8bit-mixed-13",
		"8bit-mixed-50",
		"8bit-mixed-2000",
		"16bit-mixed-13",
		"16bit-mixed-50",
		"16bit-mixed-2000"
	];

	// The 255 -> 256 flip, shown as a pair: identical shape, one value bumped over the byte ceiling.
	private static IEnumerable<PackingSample> BoundarySamples()
	{
		var bin255 = new Dimensions<ushort> { Length = 255, Width = 255, Height = 255 };
		var items255 = FiveItems(255);
		yield return SampleGenerator.Exact("boundary-255-stays-8bit", 8, bin255, items255);

		// One dimension bumped to 256, which alone forces the bin section to 16-bit.
		var bin256 = new Dimensions<ushort> { Length = 256, Width = 255, Height = 255 };
		yield return SampleGenerator.Exact("boundary-256-flips-16bit", 16, bin256, FiveItems(255));
	}

	private static Item<ushort>[] FiveItems(ushort value)
	{
		var items = new Item<ushort>[5];
		for (var index = 0; index < items.Length; index++)
		{
			items[index] = new Item<ushort>
			{
				Length = value, Width = value, Height = value,
				X = value, Y = value, Z = value
			};
		}

		return items;
	}
}
