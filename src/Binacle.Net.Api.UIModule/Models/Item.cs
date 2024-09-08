using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.Models;

public class Item : IWithDimensions, IWithQuantity
{
	public Item(int length, int width, int height) :
		this(length, width, height, 1)
	{
		
	}
	public Item(int length, int width, int height, int quantity)
	{
		this.Length = length;
		this.Width = width;
		this.Height = height;
		this.Quantity = quantity;
	}

	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int Quantity { get; set; }
	public int Volume
	{
		get
		{
			return this.CalculateVolume();
		}
		set
		{

		}
	}
}
