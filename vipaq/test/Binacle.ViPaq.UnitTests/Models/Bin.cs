using System.Numerics;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq.UnitTests.Models;

public class Bin<T> : IWithDimensions<T>
	where T: struct,
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}

