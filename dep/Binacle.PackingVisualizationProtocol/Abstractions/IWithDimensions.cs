using System.Numerics;

namespace Binacle.PackingVisualizationProtocol.Abstractions;

public interface IWithDimensions<T>
	where T: struct, INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}

