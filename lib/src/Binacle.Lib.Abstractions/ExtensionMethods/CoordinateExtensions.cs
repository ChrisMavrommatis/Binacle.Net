using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

public static class CoordinateExtensions
{
	public static string FormatCoordinates<T>(this T item)
		where T : IWithReadOnlyCoordinates
	{
		return $"({item.X},{item.Y},{item.Z})";
	}
}
