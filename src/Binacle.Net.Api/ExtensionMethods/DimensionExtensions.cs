using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Api.ExtensionMethods;

public static class DimensionExtensions
{

	public static string FormatDimensions<T>(this T item)
		where T : IWithReadOnlyDimensions
	{
		if(item is IWithQuantity withQuantity)
			return $"{item.Height}x{item.Length}x{item.Width}-{withQuantity.Quantity}";

		return $"{item.Height}x{item.Length}x{item.Width}";
	}

	public static string FormatCoordinates<T>(this T item)
		where T : IWithReadOnlyCoordinates
	{
		return $"({item.X},{item.Y},{item.Z})";
	}

	public static Dictionary<string, object> ToItemDimensionDictionary<T>(this IEnumerable<T> items)
		where T : IWithID, IWithReadOnlyDimensions
	{
		return items.ToDictionary((x) => $"{x.ID}", x => (object)x.FormatDimensions());
	}

	public static Dictionary<string, object> ToItemDimensionDictionary(this IEnumerable<ResultItem> items)
	{
		return items.ToDictionary(x => x.ID, x => (object)x.FormatDimensionsAndCoordinates());
	}

	public static string FormatDimensionsAndCoordinates(this ResultItem item)
	{
		if(item.Coordinates is not null)
			return $"{item.Dimensions.FormatDimensions()} {item.Coordinates.Value.FormatCoordinates()}";

		return $"{item.Dimensions.FormatDimensions()}";
	}

}
