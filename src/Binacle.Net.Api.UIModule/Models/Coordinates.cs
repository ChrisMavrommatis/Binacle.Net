using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.Models;

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
		X = x;
		Y = y;
		Z = z;
	}
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
}
