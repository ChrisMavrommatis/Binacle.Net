using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq.UnitTests.Models;

public class Coordinates<T> : IWithCoordinates<T>
	where T: struct,
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}

public static class Coordinates
{
	public static Coordinates<T> Create<T>(T x, T y, T z)
		where T : struct,
		IBinaryInteger<T>,
		IComparable<T>,
		INumber<T>
	{
		return new Coordinates<T>
		{
			X = x,
			Y = y,
			Z = z
		};
	}
}
