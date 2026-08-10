using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Providers;

public static class SpecializedScalingProblemsProvider
{
	public const string MaxSizeBin = "60x40x40";
	
	public static Scenario GetBaseline()
	{
		return Scenario.Create(
			name: "SpecializedBaseline",
			bin: "60x40x10",
			items: ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]"],
			metrics: "10572 24000 13 44.05",
			result: "FullyPacked FullyPacked"
		);
	}
	
    // Keyed by bin count - each entry is the one before it plus the next taller bin.
    //
    //   bin        volume
    //   60x40x10    24000
    //   60x40x15    36000
    //   60x40x20    48000
    //   60x40x25    60000
    //   60x40x30    72000
    //   60x40x35    84000
    //   60x40x40    96000   <- MaxSizeBin
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
		if (!binsByQuantity.TryGetValue(binCount, out var bins))
		{
			throw new ArgumentException($"Invalid bin count. Value {binCount} should be between 1 and 7.");
		}
		return bins.Select(TestBin.FromCompactString).ToList();
	}

    // Keyed by the running item count - each entry is the one before it plus the next item type,
    // which is why the keys look arbitrary. The last two columns are those running totals.
    //
    //   item        qty  unit vol  row vol  items  total vol
    //   2x5x10        3       100      300      3        300
    //   12x15x10      4      1800     7200      7       7500
    //   8x8x8         6       512     3072     13      10572   <- the baseline scenario; fits every bin
    //   5x5x15        4       375     1500     17      12072
    //   10x8x8        6       640     3840     23      15912
    //   4x4x4         6        64      384     29      16296
    //   2x15x5        8       150     1200     37      17496
    //   10x9x1       10        90      900     47      18396
    //   10x10x10     12      1000    12000     59      30396
    //   17x15x15      8      3825    30600     67      60996
    //   16x10x7      12      1120    13440     79      74436   <- all but BFD fail on the max bin

    private static Dictionary<int, string[]> itemsByQuantity = new Dictionary<int, string[]>()
    {
	    { 3,  ["2x5x10 [3]"] },
	    { 7,  ["2x5x10 [3]", "12x15x10 [4]"] },
	    { 13, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]"] },
	    { 17, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]"] },
	    { 23, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]", "10x8x8 [6]"] },
	    { 29, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]", "10x8x8 [6]", "4x4x4 [6]"] },
	    { 37, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]", "10x8x8 [6]", "4x4x4 [6]", "2x15x5 [8]"] },
	    { 47, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]", "10x8x8 [6]", "4x4x4 [6]", "2x15x5 [8]", "10x9x1 [10]"] },
	    { 59, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]", "10x8x8 [6]", "4x4x4 [6]", "2x15x5 [8]", "10x9x1 [10]", "10x10x10 [12]"] },
	    { 67, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]", "10x8x8 [6]", "4x4x4 [6]", "2x15x5 [8]", "10x9x1 [10]", "10x10x10 [12]", "17x15x15 [8]"] },
	    { 79, ["2x5x10 [3]", "12x15x10 [4]", "8x8x8 [6]", "5x5x15 [4]", "10x8x8 [6]", "4x4x4 [6]", "2x15x5 [8]", "10x9x1 [10]", "10x10x10 [12]", "17x15x15 [8]", "16x10x7 [12]"] },
    };
    
    public static List<TestItem> GetItems(int itemCount)
	{
	    if (!itemsByQuantity.ContainsKey(itemCount))
	    {
		    throw new ArgumentException($"Invalid item count. Value {itemCount} should be between 3 and 79.");
	    }
	    return itemsByQuantity[itemCount].Select(TestItem.FromCompactString).ToList();
	}

}
