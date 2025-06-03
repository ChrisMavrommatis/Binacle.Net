using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Fitting;

internal static class FittingAlgorithmResultBuilderExtensions
{
	internal static FittingResultBuilder<TBin, TItem> CreateResultBuilder<TBin, TItem>(
		this IFittingAlgorithm algorithm, 
		TBin bin,
		int totalItems,
		int totalItemsVolume
	)
		where TBin : IWithID, IWithReadOnlyVolume
		where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
	{
		return new FittingResultBuilder<TBin, TItem>(algorithm.AlgorithmInfo, bin, totalItems, totalItemsVolume);
	}
}
