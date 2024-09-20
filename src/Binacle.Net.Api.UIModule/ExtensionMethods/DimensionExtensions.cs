using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.ExtensionMethods;

internal static class DimensionExtensions
{
	public static string FormatDimensions<T>(this T item)
		where T : IWithReadOnlyDimensions
	{
		if (item is IWithQuantity withQuantity)
			return $"{item.Length}x{item.Width}x{item.Height}-{withQuantity.Quantity}";

		return $"{item.Length}x{item.Width}x{item.Height}";
	}
}
