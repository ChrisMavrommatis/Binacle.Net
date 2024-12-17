using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.v3.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class Coordinates : IWithCoordinates
{
	public Coordinates()
	{

	}

	public Coordinates(IWithReadOnlyCoordinates item)
		: this(item.X, item.Y, item.Z)
	{

	}

	public Coordinates(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
}
