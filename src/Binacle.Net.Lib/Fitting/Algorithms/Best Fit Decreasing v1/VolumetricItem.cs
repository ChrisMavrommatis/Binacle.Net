﻿using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class BestFitDecreasing_v1<TBin, TItem>
{
	private class VolumetricItem : IWithReadOnlyDimensions, IWithReadOnlyVolume
	{
		internal VolumetricItem(IWithReadOnlyDimensions item) :
			this(item.Length, item.Width, item.Height)
		{
		}

		internal VolumetricItem(int length, int width, int height)
		{
			this.Length = length;
			this.Width = width;
			this.Height = height;
			this.Volume = this.CalculateVolume();
			this.LongestDimension = this.CalculateLongestDimension();

		}
		public int Length { get; }
		public int Width { get; }
		public int Height { get; }
		public int Volume { get; set; }
		public int LongestDimension { get; }
	}
}
