using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v2<TBin, TItem>
{
	private sealed class Bin : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
	{
		private TBin bin;

		internal Bin(TBin bin)
		{
			this.bin = bin;
			this.Volume = bin.CalculateVolume();
			this.LongestDimension = this.CalculateLongestDimension();
		}

		public string ID { get => this.bin.ID; set => throw new NotSupportedException(); }

		public int Length => this.bin.Length;
		public int Width => this.bin.Width;
		public int Height => this.bin.Height;
		public int Volume { get; }
		public int LongestDimension { get; }
	}
}
