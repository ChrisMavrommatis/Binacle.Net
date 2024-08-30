using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Models;

public struct Coordinates : IWithReadOnlyCoordinates
{

	public static Coordinates Zero = new Coordinates(0, 0, 0);

	public int X { get; }
	public int Y { get; }
	public int Z { get; }

	public Coordinates(IWithReadOnlyCoordinates coordinates) :
		this(coordinates.X, coordinates.Y, coordinates.Z)
	{
	}

	public Coordinates(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public override string ToString()
	{
		return $"{X},{Y},{Z}";
	}
}
