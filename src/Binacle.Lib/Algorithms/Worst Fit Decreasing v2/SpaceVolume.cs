using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Models;

namespace Binacle.Lib.Algorithms;

internal partial class WorstFitDecreasing_v2<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	private class SpaceVolume : IWithReadOnlyVolume
	{
		public SpaceVolume(
			Bin bin,
			Coordinates coordinates
		) : this(new Dimensions(bin), coordinates)
		{
		}

		public SpaceVolume(
			Dimensions dimensions,
			Coordinates coordinates
		)
		{
			this.Dimensions = dimensions;
			this.Coordinates = coordinates;
			this.Volume = dimensions.CalculateVolume();
		}
		public Dimensions Dimensions { get; }
		public Coordinates Coordinates { get; }
		public int Volume { get; }
	}
}
