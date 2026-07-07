using Binacle.ViPaq.TestsKernel.Models;
// [REVIEW-VIPAQ_TEST]
namespace Binacle.ViPaq.TestsKernel.Samples;

// Builds one PackingSample from a fixed recipe. Every value comes from a seeded Random, so the same
// name always yields the same bytes — the ruler must not move between runs.
//
// Two rules the whole benchmark leans on:
//  - Every value is >= 1. Protobuf omits fields equal to 0 for free; ViPaq stores them at full width.
//    Allowing zeros would hand protobuf an unfair size win, so we never emit one.
//  - 8-bit samples keep every value <= 255 so ViPaq picks 8-bit for all three sections. 16-bit samples
//    push the bin past 255 so at least the bin section needs two bytes.
public static class SampleGenerator
{
	public static PackingSample Generate(
		string name,
		int widthBits,
		string spread,
		int itemCount,
		int seed
	)
	{
		var random = new Random(seed);

		var (itemMin, itemMax) = ItemRange(widthBits, spread);
		var (binMin, binMax) = BinRange(widthBits);

		var bin = new Dimensions<ushort>
		{
			Length = Next(random, binMin, binMax),
			Width = Next(random, binMin, binMax),
			Height = Next(random, binMin, binMax)
		};

		var items = new Item<ushort>[itemCount];
		for (var index = 0; index < itemCount; index++)
		{
			var length = Next(random, itemMin, itemMax);
			var width = Next(random, itemMin, itemMax);
			var height = Next(random, itemMin, itemMax);
			items[index] = new Item<ushort>
			{
				Length = length,
				Width = width,
				Height = height,
				// Coordinates ride inside the bin, so they follow the bin's width and stay >= 1.
				X = Next(random, 1, bin.Length),
				Y = Next(random, 1, bin.Width),
				Z = Next(random, 1, bin.Height)
			};
		}

		return new PackingSample
		{
			Name = name,
			Bin = bin,
			Items = items,
			WidthBits = widthBits,
			Spread = spread
		};
	}

	// A one-off sample with exact values, for the 255 -> 256 boundary where 8-bit flips to 16-bit.
	public static PackingSample Exact(string name, int widthBits, Dimensions<ushort> bin, Item<ushort>[] items)
		=> new()
		{
			Name = name,
			Bin = bin,
			Items = items,
			WidthBits = widthBits,
			Spread = "boundary"
		};

	private static (ushort min, ushort max) ItemRange(int widthBits, string spread)
		=> (widthBits, spread) switch
		{
			(8, "low") => (1, 60),
			(8, "high") => (200, 255),
			(8, "mixed") => (1, 255),
			(16, "low") => (256, 2_000),
			(16, "high") => (40_000, 65_535),
			(16, "mixed") => (1, 65_535),
			_ => throw new ArgumentException($"No range for {widthBits}-bit '{spread}'.")
		};

	// The bin is the container. 8-bit keeps it under 256; 16-bit pushes it well past, which forces the
	// bin section (and the coordinates that live in it) to two bytes.
	private static (ushort min, ushort max) BinRange(int widthBits)
		=> widthBits == 8 ? ((ushort)150, (ushort)255) : ((ushort)30_000, (ushort)65_535);

	private static ushort Next(Random random, ushort min, ushort max)
		=> (ushort)random.Next(min, max + 1);
}
