using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Models;

public class DimensionsAndQuantity : IWithDimensions, IWithQuantity
{
	public DimensionsAndQuantity()
	{

	}
	
	public DimensionsAndQuantity(int length, int width, int height, int quantity)
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
}
