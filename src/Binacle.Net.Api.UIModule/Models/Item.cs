using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.Models;

internal class Item : IWithReadOnlyDimensions, IWithQuantity
{
	public Item(string id, IWithReadOnlyDimensions item)
		: this(id, item.Length, item.Width, item.Height, 1)
	{

	}
	public Item(string id, IWithReadOnlyDimensions item, int quantity)
		: this(id, item.Length, item.Width, item.Height, quantity)
	{

	}

	public Item(string id, int length, int width, int height, int quantity)
	{
		ID = id;
		Length = length;
		Width = width;
		Height = height;
		Quantity = quantity;
	}

	public string ID { get; set; }
	public int Length { get; }

	public int Width { get; }

	public int Height { get; }
	public int Quantity { get; set; }
}
