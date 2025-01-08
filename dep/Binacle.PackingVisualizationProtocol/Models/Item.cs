using System.Numerics;
using Binacle.PackingVisualizationProtocol.Abstractions;

namespace Binacle.PackingVisualizationProtocol.Models;

public struct Item<T> :  IWithDimensions<T>,  IWithCoordinates<T>
	where T: struct, INumber<T>
{
	public T Length { get; set; }
	public T Width { get; set; }
	public T Height { get; set; }
	public T X { get; set; }
	public T Y { get; set; }
	public T Z { get; set; }
}
