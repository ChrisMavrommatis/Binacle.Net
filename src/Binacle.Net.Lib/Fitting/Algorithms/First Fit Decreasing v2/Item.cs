using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v2
{
	private sealed class Item : IWithID, IWithDimensions, IWithReadOnlyVolume
	{
		
		private readonly IWithReadOnlyDimensions item;
		private ushort currentOrientation;
		internal static ushort TotalOrientations = 6;

		internal Item(string id, IWithReadOnlyDimensions item) 
		{
			this.ID = id;
			this.item = item;
			this.currentOrientation = 0;
			this.Length = this.item.Length;
			this.Width = this.item.Width;
			this.Height = this.item.Height;
			this.Volume = this.CalculateVolume();
			this.LongestDimension = this.CalculateLongestDimension();
		}

		

		public string ID { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Volume { get; }
		public int LongestDimension { get; }

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
					this.Length = this.item.Length;
					this.Width = this.item.Width;
					this.Height = this.item.Height;
					break;
				// new VolumetricItem(item.Length, item.Height, item.Width)
				case 1:
					this.Length = this.item.Length;
					this.Width = this.item.Height;
					this.Height = this.item.Width;
					break;
				//new VolumetricItem(item.Width, item.Length, item.Height)
				case 2:
					this.Length = this.item.Width;
					this.Width = this.item.Length;
					this.Height = this.item.Height;
					break;
				// new VolumetricItem(item.Width, item.Height, item.Length)
				case 3:
					this.Length = this.item.Width;
					this.Width = this.item.Height;
					this.Height = this.item.Length;
					break;
				// new VolumetricItem(item.Height, item.Length, item.Width)
				case 4:
					this.Length = this.item.Height;
					this.Width = this.item.Length;
					this.Height = this.item.Width;
					break;
				// new VolumetricItem(item.Height, item.Width, item.Length)
				case 5:
					this.Length = this.item.Height;
					this.Width = this.item.Width;
					this.Height = this.item.Length;
					break;

			}

			return this;
		}
	}
}
