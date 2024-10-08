using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v3<TBin, TItem>
{
	private class VolumetricItem : IWithReadOnlyDimensions, IWithReadOnlyVolume
	{
		internal VolumetricItem(IWithReadOnlyDimensions item)
		{
			this.Length = item.Length;
			this.Width = item.Width;
			this.Height = item.Height;
			this.Volume = item.CalculateVolume();
		}

		internal VolumetricItem(int length, int width, int height)
		{
			this.Length = length;
			this.Width = width;
			this.Height = height;
			this.Volume = this.CalculateVolume();
		}
		public int Length { get; }
		public int Width { get; }
		public int Height { get; }
		public int Volume { get; }
	}
}
