using System.Numerics;

namespace Binacle.ViPaq.Abstractions;

public interface IWithDimensions<T>
	where T: struct, 
	IBinaryInteger<T>,
	IComparable<T>,
	INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}
