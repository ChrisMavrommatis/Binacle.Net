using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.UIModule.Models;

internal class Coordinates : IWithCoordinates
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
