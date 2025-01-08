using System.Numerics;

namespace Binacle.PackingVisualizationProtocol.Abstractions;

public interface IWithCoordinates<T>
	where T: struct, INumber<T>
{
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}
