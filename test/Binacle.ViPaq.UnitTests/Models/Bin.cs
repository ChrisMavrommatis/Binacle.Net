using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq.UnitTests.Models;

internal class Bin<T> : IWithDimensions<T>
	where T: struct, 
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}

public class Item<T> : IWithDimensions<T>, IWithCoordinates<T>
	where T : struct,
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}
