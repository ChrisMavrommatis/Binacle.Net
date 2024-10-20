using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Packing.Algorithms;

internal partial class BestFitDecreasing_v1<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	private sealed class Bin : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
	{

		internal Bin(TBin bin)
		{
			this.ID = bin.ID;
			this.Length = bin.Length;
			this.Width = bin.Width;
			this.Height = bin.Height;
			this.Volume = bin.CalculateVolume();
			this.LongestDimension = this.CalculateLongestDimension();
		}

		public string ID { get; set; }

		public int Length { get; }
		public int Width { get; }
		public int Height { get; }
		public int Volume { get; }
		public int LongestDimension { get; }
	}
}
