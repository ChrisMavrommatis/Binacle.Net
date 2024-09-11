using Binacle.Net.Lib.Abstractions.Models;
using System.Numerics;

namespace Binacle.Net.TestsKernel.Models;

public class Dimensions : IWithDimensions
{
	public Dimensions()
	{

	}

	public Dimensions(int length, int width, int height)
	{
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}

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
