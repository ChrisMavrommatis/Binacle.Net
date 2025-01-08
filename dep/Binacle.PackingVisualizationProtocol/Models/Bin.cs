using System.Numerics;
using Binacle.PackingVisualizationProtocol.Abstractions;

namespace Binacle.PackingVisualizationProtocol.Models;

public struct Bin<T> : IWithDimensions<T>
	where T: struct, INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
}
