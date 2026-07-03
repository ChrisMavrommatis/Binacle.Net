using System.Numerics;

namespace Binacle.CompactNotation;

// A concrete "LxWxH". What ParseDimensions returns.
public class Dimensions<T> : IWithDimensions<T>
	where T : struct, INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}
