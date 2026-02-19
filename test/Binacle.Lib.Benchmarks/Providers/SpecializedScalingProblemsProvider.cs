using Binacle.TestsKernel.Helpers;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Providers;

public static class SpecializedScalingProblemsProvider
{
	public static Scenario GetBaseline()
	{
		return Scenario.Create(
			name: "SpecializedBaseline",
			bin: "60x40x10",
			items: ["2x5x10-3", "12x15x10-4", "8x8x8-6"],
			metrics: "10572 24000 13 44.05",
			result: "FullyPacked FullyPacked"
		);
	}
	
    private static TestBin CreateBin(string bin)
    {
        var dimensions = DimensionsHelper.ParseFromCompactString(bin);
        return new TestBin(bin, dimensions);
    }

    // bins 
    // 60x40x10 => [24000]
    // 60x40x15 => [36000]
    // 60x40x20 => [48000]
    // 60x40x25 => [60000]
    // 60x40x30 => [72000]
    // 60x40x35 => [84000]
    // 60x40x40 => [96000]
    private static Dictionary<int, string[]> binsByQuantity = new Dictionary<int, string[]>()
    {
        {1, ["60x40x10"]},
        {2, ["60x40x10", "60x40x15"]},
        {3, ["60x40x10", "60x40x15", "60x40x20"]},
        {4, ["60x40x10", "60x40x15", "60x40x20", "60x40x25"]},
        {5, ["60x40x10", "60x40x15", "60x40x20", "60x40x25", "60x40x30"]},
        {6, ["60x40x10", "60x40x15", "60x40x20", "60x40x25", "60x40x30", "60x40x35"]},
        {7, ["60x40x10", "60x40x15", "60x40x20", "60x40x25", "60x40x30", "60x40x35", "60x40x40"]}
    };
    
    public static List<TestBin> GetBins(int binCount)
	{
		if (!binsByQuantity.ContainsKey(binCount))
		{
			throw new ArgumentException($"Invalid bin count. Value {binCount} should be between 1 and 7.");
		}
		return binsByQuantity[binCount].Select(CreateBin).ToList();
	}

    // items 
    // 2x5x10-3    = 3  (100x3)   [300]
    // 12x15x10-4  = 7  (1800x4)  [7200]
    // 8x8x8-6     = 13 (512x6)   [3072]   => FIT TO ALL
    //     ----------------------------------
    //               13           [10572]
    // 5x5x15-4	   = 17  (375x4)  [1500]
    // 10x8x8-6	   = 23  (640x6)  [3840]
    // 4x4x4-6	   = 29  (64x6)   [684]
    // 2x15x5-8    = 37  (150x8)  [1200]
    // 10x9x1-10   = 47  (90x10)  [900]
    // 10x10x10-12 = 59  (1000x12)[12000]
    // 17x15x15-8  = 67  (3825x8) [30600]
    // 16x10x7-12  = 79  (1120x12)[13440] => All But BFD On Max fail
    // ----------------------------------
    //               79		      [74736]

    private static TestItem CreateItem(string item)
    {
	    var dimensions = DimensionsHelper.ParseFromCompactString(item);
	    return new TestItem(item, dimensions, dimensions.Quantity);
    }
    private static Dictionary<int, string[]> itemsByQuantity = new Dictionary<int, string[]>()
    {
	    { 3,  ["2x5x10-3"] },
	    { 7,  ["2x5x10-3", "12x15x10-4"] },
	    { 13, ["2x5x10-3", "12x15x10-4", "8x8x8-6"] },
	    { 17, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4"] },
	    { 23, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4", "10x8x8-6"] },
	    { 29, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4", "10x8x8-6", "4x4x4-6"] },
	    { 37, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4", "10x8x8-6", "4x4x4-6", "2x15x5-8"] },
	    { 47, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4", "10x8x8-6", "4x4x4-6", "2x15x5-8", "10x9x1-10"] },
	    { 59, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4", "10x8x8-6", "4x4x4-6", "2x15x5-8", "10x9x1-10", "10x10x10-12"] },
	    { 67, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4", "10x8x8-6", "4x4x4-6", "2x15x5-8", "10x9x1-10", "10x10x10-12", "17x15x15-8"] },
	    { 79, ["2x5x10-3", "12x15x10-4", "8x8x8-6", "5x5x15-4", "10x8x8-6", "4x4x4-6", "2x15x5-8", "10x9x1-10", "10x10x10-12", "17x15x15-8", "16x10x7-12"] },
    };
    
    public static List<TestItem> GetItems(int itemCount)
	{
	    if (!itemsByQuantity.ContainsKey(itemCount))
	    {
		    throw new ArgumentException($"Invalid item count. Value {itemCount} should be between 3 and 79.");
	    }
	    return itemsByQuantity[itemCount].Select(CreateItem).ToList();
	}
    
    

}
