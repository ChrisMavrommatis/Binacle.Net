using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Models;

namespace Binacle.Lib.Algorithms;

internal partial class FirstFitDecreasing_v2<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	private sealed class Item : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
	{
		internal const ushort TotalOrientations = 6;

		private readonly TItem originalItem;

		private ushort currentOrientation;
		private Coordinates? coordinates;


		internal Item(TItem item, int volume, int longestDimension)
		{
			this.originalItem = item;
			this.currentOrientation = 0;
			this.Length = item.Length;
			this.Width = item.Width;
			this.Height = item.Height;
			this.Volume = volume;
			this.LongestDimension = longestDimension;
			this.IsPacked = false;
		}
		public string ID 
		{ 
			get
			{
				return this.originalItem.ID;
			}
			set 
			{
				this.originalItem.ID = value;
			}
		}
		public int Volume { get; }
		public int Length { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public int X => this.coordinates!.Value.X;
		public int Y => this.coordinates!.Value.Y;
		public int Z => this.coordinates!.Value.Z;

		public int LongestDimension { get; }
		public bool IsPacked { get; private set; }

		internal Item Rotate()
		{
			if (this.currentOrientation >= TotalOrientations)
				this.currentOrientation = 0;
			else
				this.currentOrientation++;

			// The six permutations of the box's three axes. Case 0 is the item as it came in, and
			// orientation 6 folds back onto it, so a full six-call sweep ends where it started.
			switch (this.currentOrientation)
			{
				case 0:
				default:
					this.Length = this.originalItem.Length;
					this.Width = this.originalItem.Width;
					this.Height = this.originalItem.Height;
					break;
				case 1:
					this.Length = this.originalItem.Length;
					this.Width = this.originalItem.Height;
					this.Height = this.originalItem.Width;
					break;
				case 2:
					this.Length = this.originalItem.Width;
					this.Width = this.originalItem.Length;
					this.Height = this.originalItem.Height;
					break;
				case 3:
					this.Length = this.originalItem.Width;
					this.Width = this.originalItem.Height;
					this.Height = this.originalItem.Length;
					break;
				case 4:
					this.Length = this.originalItem.Height;
					this.Width = this.originalItem.Length;
					this.Height = this.originalItem.Width;
					break;
				case 5:
					this.Length = this.originalItem.Height;
					this.Width = this.originalItem.Width;
					this.Height = this.originalItem.Length;
					break;
			}

			return this;
		}

		internal void Pack(SpaceVolume spaceQuadrant)
		{
			this.IsPacked = true;
			this.coordinates = spaceQuadrant.Coordinates;
		}



	}
}
