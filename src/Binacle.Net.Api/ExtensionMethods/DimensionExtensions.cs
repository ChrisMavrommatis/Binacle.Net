﻿using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Api.ExtensionMethods;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class DimensionExtensions
{
	public static string FormatDimensions<T>(this T item)
		where T : IWithReadOnlyDimensions
	{
		if(item is IWithQuantity withQuantity)
			return $"{item.Length}x{item.Width}x{item.Height}-{withQuantity.Quantity}";

		return $"{item.Length}x{item.Width}x{item.Height}";
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

	public static string FormatDimensionsAndCoordinates(this ResultItem item)
	{
		if(item.Coordinates is not null)
			return $"{item.Dimensions.FormatDimensions()} {item.Coordinates.Value.FormatCoordinates()}";

		return $"{item.Dimensions.FormatDimensions()}";
	}

}
