using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Fitting.Algorithms;

internal sealed partial class BestFitDecreasing_v1<TBin, TItem>
{
	private sealed class Bin : VolumetricItem, IWithID
	{
		public Bin(string id, IWithReadOnlyDimensions item) :
			base(item)
		{
			this.ID = id;
		}

		public Bin(string id, int length, int width, int height) :
			base(length, width, height)
		{
			this.ID = id;
		}

		public string ID { get; set; }
	}
}
