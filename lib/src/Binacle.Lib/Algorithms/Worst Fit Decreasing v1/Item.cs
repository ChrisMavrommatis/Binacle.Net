using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Algorithms;

internal partial class WorstFitDecreasing_v1<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	private sealed class Item : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
	{
		internal const ushort TotalOrientations = 6;

		private readonly int originalLength;
		private readonly int originalWidth;
		private readonly int originalHeight;

		private ushort currentOrientation;

		internal Item(TItem item)
		{
			this.currentOrientation = 0;
			this.ID = item.ID;

			this.originalLength = item.Length;
			this.originalWidth = item.Width;
			this.originalHeight = item.Height;

			this.Length = item.Length;
			this.Width = item.Width;
			this.Height = item.Height;

			this.Volume = this.CalculateVolume();
			this.LongestDimension = this.CalculateLongestDimension();
			this.IsPacked = false;
		}
		public string ID { get; set; }
		
		public int Volume { get; }
		public int Length { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public int X { get; private set; }
		public int Y { get; private set; }
		public int Z { get; private set; }

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
					this.Length = this.originalLength;
					this.Width = this.originalWidth;
					this.Height = this.originalHeight;
					break;
				case 1:
					this.Length = this.originalLength;
					this.Width = this.originalHeight;
					this.Height = this.originalWidth;
					break;
				case 2:
					this.Length = this.originalWidth;
					this.Width = this.originalLength;
					this.Height = this.originalHeight;
					break;
				case 3:
					this.Length = this.originalWidth;
					this.Width = this.originalHeight;
					this.Height = this.originalLength;
					break;
				case 4:
					this.Length = this.originalHeight;
					this.Width = this.originalLength;
					this.Height = this.originalWidth;
					break;
				case 5:
					this.Length = this.originalHeight;
					this.Width = this.originalWidth;
					this.Height = this.originalLength;
					break;
			}

			return this;
		}

		internal void Pack(SpaceVolume spaceQuadrant)
		{
			this.IsPacked = true;
			this.X = spaceQuadrant.Coordinates.X;
			this.Y = spaceQuadrant.Coordinates.Y;
			this.Z = spaceQuadrant.Coordinates.Z;
		}

	}
}
