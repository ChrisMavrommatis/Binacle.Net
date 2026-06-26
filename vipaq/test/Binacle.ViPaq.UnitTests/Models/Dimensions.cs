using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq.UnitTests.Models;

public class Dimensions<T> : IWithDimensions<T>
	where T: struct,
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}

public static class Dimensions
{
	public static Dimensions<T> Create<T>(T length, T width, T height)
		where T: struct,
		IBinaryInteger<T>,
		IComparable<T>,
		INumber<T>
	{
		return new Dimensions<T>
		{
			Length = length,
			Width = width,
			Height = height
		};
	}
}

