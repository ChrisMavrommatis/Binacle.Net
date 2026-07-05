using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel.Helpers;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.ResultSelection.Helpers;

internal static class OperationResultHelper
{
	public static OperationResult ParseFromCompactString(string compactString)
	{
		// "60x40x10 FFD_v2 PartiallyPacked 72.13 96.11",
		// "60x40x10 BFD_v2 FullyPacked 75.05 100",
		// "60x40x10 WFD_v2 PartiallyPacked 69.22"
		
		var parts = compactString.Split(' ');
		if (parts.Length != 5)
		{
			throw new FormatException($"Invalid compact string format: {compactString}");
		}
		var bin = TestBin.FromCompactString(parts[0]);
		var algorithmInfo = AlgorithmInfoHelper.ParseFromCompactString(parts[1]);
		var status = Enum.Parse<OperationResultStatus>(parts[2]);
		var binPct = decimal.Parse(parts[3]);
		var itemsPct = decimal.Parse(parts[4]);

		var operationResult = Create(
			new PackedBin(parts[0], bin),
			algorithmInfo,
			status,
			binPct,
			itemsPct
		);

		return operationResult;
	}
	
    public static Dictionary<string, OperationResult> ParseManyFromCompactStrings(Dictionary<string, string> compactStrings)
	{
		var results = new Dictionary<string, OperationResult>();
		foreach (var (key, compactString) in compactStrings)
		{
			var result = ParseFromCompactString(compactString);
			results.Add(key, result);
		}
		return results;
	}
    
    public static OperationResult Create(
	    PackedBin bin,
		AlgorithmInfo algorithmInfo,
		OperationResultStatus status,
		decimal binPct,
		decimal itemsPct
	)
	{
		return new OperationResult()
		{
			Bin = bin,
			AlgorithmInfo = algorithmInfo,
			AlgorithmOperation = AlgorithmOperation.Packing,
			Status = status,
			PackedItems = Enumerable.Empty<PackedItem>().ToList().AsReadOnly(),
			UnpackedItems = Enumerable.Empty<UnpackedItem>().ToList().AsReadOnly(),
			PackedBinVolumePercentage = binPct,
			PackedItemsVolumePercentage = itemsPct
		};
	}
}
