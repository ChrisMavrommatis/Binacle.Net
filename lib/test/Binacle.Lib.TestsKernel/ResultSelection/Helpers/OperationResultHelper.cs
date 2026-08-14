using Binacle.CompactNotation;

namespace Binacle.Lib.TestsKernel.ResultSelection.Helpers;

internal static class OperationResultHelper
{
	public static OperationResult ParseFromCompactString(string compactString)
	{
		// "<bin> <algorithm> <status> <bin volume %> <items volume %>"
		// e.g. "60x40x30 FFD_v2 FullyPacked 95 100".
		var parts = compactString.Split(' ');
		if (parts.Length != 5)
		{
			throw new FormatException($"Invalid compact string format: {compactString}");
		}
		// PackedBin wants the non-generic IWithReadOnlyDimensions, which the parser's Dimensions<int> does not
		// implement. Binacle.Packing's internal Dimensions does, and this project is a friend.
		var parsed = CompactNotationParser.ParseDimensions<int>(parts[0]);
		var bin = new Binacle.Packing.Dimensions(parsed.Length, parsed.Width, parsed.Height);
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
