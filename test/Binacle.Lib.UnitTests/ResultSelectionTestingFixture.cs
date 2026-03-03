using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel.Helpers;
using Xunit.Internal;

namespace Binacle.Lib.UnitTests;

public sealed class ResultSelectionTestingFixture : IDisposable
{
	
	public ResultSelectionTestingFixture()
	{
		
	}

	public void Dispose()
	{
	}
	
	public OperationResult MakeResult(
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
			AlgorithmOperation =  AlgorithmOperation.Packing,
			Status = status,
			PackedItems = Enumerable.Empty<PackedItem>().CastOrToReadOnlyList(),
			UnpackedItems = Enumerable.Empty<UnpackedItem>().CastOrToReadOnlyList(),
			PackedBinVolumePercentage = binPct,
			PackedItemsVolumePercentage = itemsPct
		};
	}
}
