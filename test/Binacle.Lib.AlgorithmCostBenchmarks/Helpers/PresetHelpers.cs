using Binacle.Lib.AlgorithmCostBenchmarks.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.AlgorithmCostBenchmarks.Helpers;

internal static class PresetHelper
{
	public static (int Min, int Max) GetBinDimensionRange(BinSize size)
	{
		return size switch
		{
			BinSize.Small => (25, 75),
			BinSize.Medium => (80, 175),
			BinSize.Large => (190, 445),
			BinSize.XLarge => (500, 980),
			_ => throw new ArgumentException($"Unknown bin size: {size}")
		};
	}
	
	public static (int Min, int Max) GetItemDimensionRange(BinSize binSize, ItemSizeDistribution distribution)
	{
		var binRange = GetBinDimensionRange(binSize);
		int maxBinDim = binRange.Max;
        
		return distribution switch
		{
			ItemSizeDistribution.Uniform => ((int)(maxBinDim * 0.1), (int)(maxBinDim * 0.9)),
			ItemSizeDistribution.Mixed => ((int)(maxBinDim * 0.1), (int)(maxBinDim * 0.9)),
			ItemSizeDistribution.MostlySmall => ((int)(maxBinDim * 0.1), (int)(maxBinDim * 0.9)),
			ItemSizeDistribution.MostlyLarge => ((int)(maxBinDim * 0.1), (int)(maxBinDim * 0.9)),
			_ => throw new ArgumentException($"Unknown distribution: {distribution}")
		};
	}
	
}


internal static class ConstructionHelper
{
    public static TestBin[] GenerateBins(Random random, int binCount, BinSize binSize)
    {
	    var bins = new TestBin[binCount];
	    var range = PresetHelper.GetBinDimensionRange(binSize);

	    for (int i = 0; i < binCount; i++)
	    {
		    bins[i] = new TestBin
		    {
			    ID = $"Bin_{i}",
			    Length = random.Next(range.Min, range.Max + 1),
			    Width = random.Next(range.Min, range.Max + 1),
			    Height = random.Next(range.Min, range.Max + 1)
		    };
	    }

	    return bins;
    }
    
    public static TestItem[] GenerateItems(
	    Random random, 
	    int itemCount,
	    ItemSizeDistribution distribution, 
	    BinSize binSize)
    {
	    var binRange = PresetHelper.GetBinDimensionRange(binSize);

	    // Item dimensions are 5% to 90% of max bin dimension
	    int minDim = Math.Max(1, (int)(binRange.Min * 0.2));
	    int maxDim = (int)(binRange.Max * 0.9);

	    var itemList = new List<TestItem>();
	    int remainingCount = itemCount;
	    int itemIndex = 0;

	    while (remainingCount > 0)
	    {
		    // Quantity between 1 and min(10, remainingCount)
		    int maxQty = Math.Min(10, remainingCount);
		    int quantity = random.Next(1, maxQty + 1);

		    var dims = GenerateDimensions(random, distribution, minDim, maxDim);
		    itemList.Add(new TestItem
		    {
			    ID = $"Item_{itemIndex}",
			    Length = dims.L,
			    Width = dims.W,
			    Height = dims.H,
			    Quantity = quantity
		    });

		    remainingCount -= quantity;
		    itemIndex++;
	    }

	    return itemList.ToArray();
    }
    
    private static (int L, int W, int H) GenerateDimensions(
	    Random r, ItemSizeDistribution dist, int min, int max) =>
	    dist switch
	    {
		    ItemSizeDistribution.Uniform => Uniform(r, min, max),
		    ItemSizeDistribution.Mixed => Mixed(r, min, max),
		    ItemSizeDistribution.MostlySmall => Biased(r, min, max, 0.35, smallBias: true),
		    ItemSizeDistribution.MostlyLarge => Biased(r, min, max, 0.60, smallBias: false),
		    _ => throw new ArgumentException($"Unknown distribution: {dist}")
	    };
    
    
    private static (int, int, int) Uniform(Random r, int min, int max) =>
	    (r.Next(min, max + 1), r.Next(min, max + 1), r.Next(min, max + 1));

    private static (int, int, int) Mixed(Random r, int min, int max)
    {
	    double p = r.NextDouble();
	    int range = max - min;
        
	    var (lo, hi) = p < 0.5 
		    ? (min, min + (int)(range * 0.4))
		    : p < 0.8 
			    ? (min + (int)(range * 0.4), min + (int)(range * 0.7))
			    : (min + (int)(range * 0.7), max);

	    return (r.Next(lo, hi + 1), r.Next(lo, hi + 1), r.Next(lo, hi + 1));
    }

    private static (int, int, int) Biased(Random r, int min, int max, double split, bool smallBias)
    {
	    int pivot = min + (int)((max - min) * split);
	    bool primary = r.NextDouble() < 0.8;

	    var (lo, hi) = (smallBias, primary) switch
	    {
		    (true, true) => (min, pivot),
		    (true, false) => (pivot, max),
		    (false, true) => (pivot, max),
		    (false, false) => (min, pivot)
	    };

	    return (r.Next(lo, hi + 1), r.Next(lo, hi + 1), r.Next(lo, hi + 1));
    }
}

