using Binacle.Net.TestsKernel.Helpers;

namespace Binacle.Net.TestsKernel.Models;

public sealed class ItemsCollection
{
	private List<TestItem> items;

	public ItemsCollection()
	{
		this.items = new();
	}

	public ItemsCollection Add(string id, int length, int width, int height, int quantity = 1)
	{
		var dimensions = new Dimensions(length, width, height);
		var item = new TestItem(id, dimensions, quantity);
		this.items.Add(item);
		return this;
	}
	public ItemsCollection Add(string itemString)
	{
		var dimensions = DimensionHelper.ParseFromCompactString(itemString);
		var item = new TestItem(itemString, dimensions, dimensions.Quantity);
		this.items.Add(item);
		return this;
	}

	public List<TestItem> GetItems()
	{
		return this.items;
	}
}