using Binacle.Geometry;
using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// Synthetic (generated) scenarios — the CPU/memory sibling of the real BischoffDataProvider /
// CustomProblemsDataProvider. Deterministic random data at item counts no real pack reaches (2000, 5000), for the
// speed/memory benchmarks only.
//
// **Never use these for size or compression.** Random data has nothing for a codec to grip, so it reports the
// *opposite* of real behaviour — size and crossover use real data only (decisions.md D9). CPU and memory, though,
// depend on item count and byte width, not on whether values repeat, so random is fine and preferred here: it
// scales freely and deliberately exercises the expensive path.
public static class SyntheticDataProvider
{
	// A fixed seed base, so every run generates byte-identical scenarios (BDN builds them in GlobalSetup, and a
	// benchmark whose input changed between runs would not be comparable).
	private const int SeedBase = 20_260_714;

	private const ushort EightBitMax = 255;
	private const ushort SixteenBitMax = 65_535;

	// Two counts past any real pack, at each width family. Spread is "mixed" — CPU and memory do not depend on
	// where the values sit, only on the count and the byte width (D9).
	private static readonly int[] Counts = [2_000, 5_000];
	private static readonly int[] WidthBitsMatrix = [8, 16];

	private static readonly Dictionary<string, Scenario> scenarios;

	static SyntheticDataProvider()
	{
		scenarios = new Dictionary<string, Scenario>();

		foreach (var widthBits in WidthBitsMatrix)
		{
			foreach (var count in Counts)
			{
				var scenario = Generate(count, widthBits);
				scenarios.Add(scenario.Name, scenario);
			}
		}
	}

	public static IReadOnlyCollection<Scenario> All => scenarios.Values;

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string name) => scenarios[name];

	private static Scenario Generate(int count, int widthBits)
	{
		// Deterministic per (count, widthBits): same seed → same bytes every run.
		var random = new Random(SeedBase + (widthBits * 100_000) + count);

		// 8-bit keeps every value in one byte; 16-bit forces two bytes by starting dimensions past 255. Bin is
		// the width max so any item value is in range.
		var maxValue = widthBits == 8 ? EightBitMax : SixteenBitMax;
		var minDimension = widthBits == 8 ? (ushort)1 : (ushort)256;

		var bin = new Dimensions<ushort>
		{
			Length = maxValue,
			Width = maxValue,
			Height = maxValue
		};

		var items = new Item<ushort>[count];
		for (var index = 0; index < count; index++)
		{
			items[index] = new Item<ushort>
			{
				Length = Next(random, minDimension, maxValue),
				Width = Next(random, minDimension, maxValue),
				Height = Next(random, minDimension, maxValue),
				X = Next(random, 0, maxValue),
				Y = Next(random, 0, maxValue),
				Z = Next(random, 0, maxValue)
			};
		}

		return new Scenario
		{
			Name = $"synthetic_{count}_{widthBits}bit",
			Bin = bin,
			Items = items,
			WidthBits = widthBits,
			Spread = "mixed"
		};
	}

	// Dimensions must be >= 1 (a zero dimension is rejected); coordinates may be 0. Both stay within the width max.
	private static ushort Next(Random random, int minInclusive, int maxInclusive)
		=> (ushort)random.Next(minInclusive, maxInclusive + 1);
}
