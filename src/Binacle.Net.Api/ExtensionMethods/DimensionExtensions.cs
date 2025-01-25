using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Api.ExtensionMethods;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class DimensionExtensions
{
	public static Dictionary<string, object> ToItemDimensionDictionary<T>(this IEnumerable<T> items)
		where T : IWithID, IWithReadOnlyDimensions
	{
		return items.ToDictionary((x) => $"{x.ID}", x => (object)x.FormatDimensions());
	}

	public static string FormatDimensionsAndCoordinates(this ResultItem item)
	{
		if(item.Coordinates is not null)
			return $"{item.Dimensions.FormatDimensions()} {item.Coordinates.Value.FormatCoordinates()}";

		return $"{item.Dimensions.FormatDimensions()}";
	}

}
