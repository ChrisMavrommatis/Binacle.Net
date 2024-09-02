using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v1
{
	private sealed class Item : ItemBase
	{
		internal Item(string id, IWithReadOnlyDimensions<int> item) : base(id, item)
		{
		}

		internal Item(string id, int length, int width, int height)
			: base(id, length, width, height)
		{
		}


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
