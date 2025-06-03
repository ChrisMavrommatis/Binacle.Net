using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Packing;

internal static class PackingAlgorithmResultBuilderExtensions
{
	internal static PackingResultBuilder<TBin, TItem> CreateResultBuilder<TBin, TItem>(
		this IPackingAlgorithm algorithm, 
		TBin bin,
		int totalItems,
		int totalItemsVolume
	)
		where TBin : IWithID, IWithReadOnlyVolume
		where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
	{
		return new PackingResultBuilder<TBin, TItem>(algorithm.AlgorithmInfo, bin, totalItems, totalItemsVolume);
	}
}
