using System.Numerics;

namespace Binacle.Geometry;

// A thing whose dimensions can be set. Consumers that write (e.g. vipaq deserialize) use this.
public interface IWithDimensions<T> : IWithReadOnlyDimensions<T>
	where T : struct, IBinaryInteger<T>
{
	new T Length { get; set; }
	new T Width { get; set; }
	new T Height { get; set; }
}
