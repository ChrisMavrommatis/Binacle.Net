using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib;

public static class DimensionExtensions
{
	public static int CalculateVolume(this IWithReadOnlyDimensions dimensions)
	{
		return dimensions.Length * dimensions.Width * dimensions.Height;
	}
	public static int CalculateLongestDimension(this IWithReadOnlyDimensions dimensions)
	{
		var longestDimension = dimensions.Length;

		if (dimensions.Width > longestDimension)
			longestDimension = dimensions.Width;

		if (dimensions.Height > longestDimension)
			longestDimension = dimensions.Height;

		return longestDimension;
	}
}
