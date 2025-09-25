using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.UIModule.Models;

internal class Dimensions : IWithDimensions
{
	public Dimensions()
	{

	}

	public Dimensions(IWithReadOnlyDimensions item)
		: this(item.Length, item.Width, item.Height)
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
