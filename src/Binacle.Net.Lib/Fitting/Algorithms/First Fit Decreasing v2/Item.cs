using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v2<TBin, TItem>
{
	private sealed class Item : IWithID, IWithDimensions, IWithReadOnlyVolume
	{
		private readonly int originalLength;
		private readonly int originalWidth;
		private readonly int originalHeight;

		private ushort currentOrientation;
		internal static ushort TotalOrientations = 6;

		internal Item(string id, IWithReadOnlyDimensions item) 
		{
			this.ID = id;
			this.currentOrientation = 0;

			this.originalLength = item.Length;
			this.originalWidth = item.Width;
			this.originalHeight = item.Height;

			this.Length = item.Length;
			this.Width = item.Width;
			this.Height = item.Height;
			this.Volume = this.CalculateVolume();


			this.LongestDimension = this.CalculateLongestDimension();
		}

		public string ID { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Volume { get; }
		public int LongestDimension { get; }
		public bool Fitted { get; set; }

		internal Item Rotate()
		{
			if (this.currentOrientation >= TotalOrientations)
				this.currentOrientation = 0;
			else
				this.currentOrientation++;

			switch (this.currentOrientation)
			{
				// VolumetricItem(item.Length, item.Width, item.Height)
				case 0:
				default:
					this.Length = this.originalLength;
					this.Width = this.originalWidth;
					this.Height = this.originalHeight;
					break;
				// new VolumetricItem(item.Length, item.Height, item.Width)
				case 1:
					this.Length = this.originalLength;
					this.Width = this.originalHeight;
					this.Height = this.originalWidth;
					break;
				//new VolumetricItem(item.Width, item.Length, item.Height)
				case 2:
					this.Length = this.originalWidth;
					this.Width = this.originalLength;
					this.Height = this.originalHeight;
					break;
				// new VolumetricItem(item.Width, item.Height, item.Length)
				case 3:
					this.Length = this.originalWidth;
					this.Width = this.originalHeight;
					this.Height = this.originalLength;
					break;
				// new VolumetricItem(item.Height, item.Length, item.Width)
				case 4:
					this.Length = this.originalHeight;
					this.Width = this.originalLength;
					this.Height = this.originalWidth;
					break;
				// new VolumetricItem(item.Height, item.Width, item.Length)
				case 5:
					this.Length = this.originalHeight;
					this.Width = this.originalWidth;
					this.Height = this.originalLength;
					break;

			}

			return this;
		}
	}
}
