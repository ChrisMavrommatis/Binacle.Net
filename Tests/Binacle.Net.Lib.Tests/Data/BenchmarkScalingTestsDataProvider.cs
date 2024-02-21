using Binacle.Net.Lib.Tests.Models;
using System.Collections;

namespace Binacle.Net.Lib.Tests.Data.Providers;

internal class BenchmarkScalingTestsDataProvider : IEnumerable<object[]>
{
    public const string BinCollectionName = "RectangularCuboids";
    private static Dictionary<int, string> testCases = new Dictionary<int, string>()
    {
        { 10, "Small" },
        { 50, "Small" },
        { 100, "Small" },
        { 190, "Small" },
        { 192, "Small" },
        { 193, "Medium" },
        { 195, "Medium" },
        { 300, "Medium" },
        { 382, "Medium" },
        { 384, "Medium" },
        { 385, "Large" },
        { 387, "Large" },
        { 475, "Large" },
        { 574, "Large" },
        { 576, "Large" },
        { 577, "None" },
        { 580, "None" },
        { 1000, "None" }
    };

    // ranges for assertion 193-384, 385-576, 577-1000
    private static Dictionary<string, Models.Range> ranges = new Dictionary<string, Models.Range>()
    {
        { "Small", new Models.Range(1, 192) },
        { "Medium", new Models.Range(193, 384) },
        { "Large", new Models.Range(385, 576) },
        { "None", new Models.Range(577, 1000) }
    };
    

    public static IEnumerable<int> GetNoOfItems()
    {
        return testCases.Keys;
    }

    public static Dimensions<int> GetDimensions() => new Dimensions<int>(5, 5, 5);

    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (var (noOfItems, expectedSize) in testCases)
        {
            yield return new object[] { noOfItems, expectedSize };
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static void AssertSuccessfulResult(Lib.Models.BinFittingOperationResult result, int noOfItems)
    {
        var foundBin = result.FoundBin?.ID ?? "None";

        if(!ranges.TryGetValue(foundBin, out var range))
        {
            throw new ApplicationException("Error. Uncaught Test Result");
        }

        if (!range.IsWithin(noOfItems))
        {
            throw new ApplicationException("Error. Invalid Test Result");
        }

    }
}
