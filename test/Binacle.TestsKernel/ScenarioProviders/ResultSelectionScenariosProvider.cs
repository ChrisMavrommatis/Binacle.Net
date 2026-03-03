using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel.Helpers;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.ScenarioProviders;

public static class ResultSelectionScenariosProvider
{
    private static readonly Dictionary<string, ResultSelectionScenario> scenarios;

    static ResultSelectionScenariosProvider()
    {
        scenarios = new Dictionary<string, ResultSelectionScenario>();

        scenarios.Add("Best Algorithm - One Fully Packed winner", new ResultSelectionScenario()
        {
            ExpectedResult = "BFD_v2",
            Results = new()
            {
                { "FFD_v2", Create("60x40x10", "FFD_v2", OperationResultStatus.PartiallyPacked, 72.13m, 96.11m) },
                { "BFD_v2", Create("60x40x10", "BFD_v2", OperationResultStatus.FullyPacked, 75.05m, 100m) },
                { "WFD_v2", Create("60x40x10", "WFD_v2", OperationResultStatus.PartiallyPacked, 69.22m, 92.23m) }
            }
        });
        
        scenarios.Add("Best Algorithm - Multiple Fully Packed", new ResultSelectionScenario()
		{
			ExpectedResult = "FFD_v2",
			Results = new()
			{
				{ "FFD_v2", Create("60x40x10", "FFD_v2", OperationResultStatus.FullyPacked, 66.55m, 100m) },
				{ "BFD_v2", Create("60x40x10", "BFD_v2", OperationResultStatus.FullyPacked, 66.55m, 100m) },
				{ "WFD_v2", Create("60x40x10", "WFD_v2", OperationResultStatus.FullyPacked, 66.55m, 100m) }
			}
		});
        
        scenarios.Add("Best Algorithm - All Partially Packed", new ResultSelectionScenario()
		{
			ExpectedResult = "FFD_v2",
			Results = new()
			{
                {"FFD_v2", Create("60x40x10", "FFD_v2", OperationResultStatus.PartiallyPacked, 78.08m, 78.23m)},
                {"BFD_v2", Create("60x40x10", "BFD_v2", OperationResultStatus.PartiallyPacked, 80.30m, 80.46m)},
                {"WFD_v2", Create("60x40x10", "WFD_v2", OperationResultStatus.PartiallyPacked, 78.08m, 78.23m)}
			}
		});
    }

    public static OperationResult Create(
        string binString,
        string algorithmString,
        OperationResultStatus status,
        decimal binPct,
        decimal itemsPct
    )
    {
        var bin = DimensionsHelper.ParseFromCompactString(binString);
        var algorithmInfo = AlgorithmInfoHelper.ParseFromCompactString(algorithmString);
        return new OperationResult()
        {
            Bin = new PackedBin(binString, bin),
            AlgorithmInfo = algorithmInfo,
            AlgorithmOperation = AlgorithmOperation.Packing,
            Status = status,
            PackedItems = Enumerable.Empty<PackedItem>().ToList().AsReadOnly(),
            UnpackedItems = Enumerable.Empty<UnpackedItem>().ToList().AsReadOnly(),
            PackedBinVolumePercentage = binPct,
            PackedItemsVolumePercentage = itemsPct
        };
    }

    public static IEnumerable<string> GetScenarioNames()
        => scenarios.Keys;

    public static IEnumerable<object[]> ScenarioNames
        => GetScenarioNames().Select(name => new object[] { name });

    public static IEnumerable<ResultSelectionScenario> GetScenarios()
        => scenarios.Values;

    public static ResultSelectionScenario GetScenarioByName(string name)
        => scenarios[name];
}
