using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.Models;

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
