using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Strategies.Models;

internal abstract class ItemBase : VolumetricItem, IItemWithReadOnlyDimensions<int>
{
	internal ItemBase(string id, IWithReadOnlyDimensions<int> item) : base(item)
	{
		ID = id;
	}

	internal ItemBase(string id, int length, int width, int height)
		: base(length, width, height)
	{
		ID = id;
	}

	public string ID { get; set; }
}
