using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.ApiModels;

public class Item : IWithReadOnlyDimensions, IWithQuantity
{
	public Item(IWithReadOnlyDimensions item)
		: this(item.Length, item.Width, item.Height, 1)
	{

	}
	public Item(IWithReadOnlyDimensions item, int quantity)
		: this(item.Length, item.Width, item.Height, quantity)
	{

	}

	public Item(int length, int width, int height, int quantity)
	{
		this.ID = $"{length}x{width}x{height}-q{quantity}";
		this.Length = length;
		this.Width = width;
		this.Height = height;
		this.Quantity = quantity;
	}

	public string ID{ get; set; }
	public int Length { get; }

	public int Width { get; }

	public int Height { get; }
	public int Quantity { get; set; }
}
