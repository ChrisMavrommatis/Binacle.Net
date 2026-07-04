using System.Numerics;

namespace Binacle.ViPaq.Abstractions;

public interface IWithDimensions<T>
	where T: struct, IBinaryInteger<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}
