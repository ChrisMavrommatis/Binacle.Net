using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Models;

public readonly struct Dimensions : IWithReadOnlyDimensions
{
	public int Length { get; }
	public int Width { get; }
	public int Height { get; }

	public Dimensions(IWithReadOnlyDimensions dimensions) :
		this(dimensions.Length, dimensions.Width, dimensions.Height)
	{
	}

	public Dimensions(int length, int width, int height)
	{
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}

	public override string ToString()
	{
		return $"{Length}x{Width}x{Height}";
	}
}
