using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Fitting.Algorithms;

internal sealed partial class BestFitDecreasing_v1<TBin, TItem>
{
	private sealed class Item : VolumetricItem, IWithID
	{
		internal Item(string id, IWithReadOnlyDimensions item) : 
			base(item)
		{
			this.ID = id;
		}

		internal Item(string id, int length, int width, int height) :
			base(length, width, height)
		{
			this.ID = id;
		}

		public string ID { get; set; }
		public bool Fitted { get; set; }

		internal IEnumerable<Item> GetOrientations()
		{
			// Length  Width  Height
			//  8      45     62
			//  8      62     45
			// 45       8     62
			// 45      62      8
			// 62       8     45
			// 62      45      8

			// L W H
			// L H W
			// W L H
			// W H L
			// H L W
			// H W L
			yield return new Item(this.ID, this.Length, this.Width, this.Height);
			yield return new Item(this.ID, this.Length, this.Height, this.Width);

			yield return new Item(this.ID, this.Width, this.Length, this.Height);
			yield return new Item(this.ID, this.Width, this.Height, this.Length);

			yield return new Item(this.ID, this.Height, this.Length, this.Width);
			yield return new Item(this.ID, this.Height, this.Width, this.Length);
		}

	}
}
