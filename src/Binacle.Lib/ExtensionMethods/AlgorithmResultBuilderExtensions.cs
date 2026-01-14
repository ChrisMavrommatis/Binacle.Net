using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Models;

namespace Binacle.Lib.ExtensionMethods;

internal static class AlgorithmResultBuilderExtensions
{
	internal static OperationResultBuilder<TBin, TItem> CreateResultBuilder<TBin, TItem>(
		this IPackingAlgorithm algorithmInstance,
		TBin bin,
		int totalItems,
		int totalItemsVolume,
		IOperationParameters parameters
	)
		where TBin : IWithID, IWithReadOnlyVolume
		where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
	{
		var algorithmInfo = new AlgorithmInfo(
			algorithmInstance.Algorithm,
			version: algorithmInstance.Version,
			operation: parameters.Operation
		);
		return new OperationResultBuilder<TBin, TItem>(algorithmInfo, bin, totalItems, totalItemsVolume, parameters);
	}
}
