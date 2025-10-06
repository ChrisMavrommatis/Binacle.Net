using System.Numerics;

namespace Binacle.ViPaq.Abstractions;

public interface IWithCoordinates<T>
	where T: struct, IBinaryInteger<T>
{
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}
