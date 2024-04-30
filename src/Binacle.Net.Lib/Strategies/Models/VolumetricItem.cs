using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Strategies.Models;

internal class VolumetricItem : VolumetricItemBase<int, int>
{
	internal VolumetricItem(IWithReadOnlyDimensions<int> item) : base(item)
	{

	}

	internal VolumetricItem(int length, int width, int height) : base(length, width, height)
	{
	}

	internal override int CalculateLongestDimension()
	{
		var largestDimension = length;

		if (width > largestDimension)
			largestDimension = width;

		if (height > largestDimension)
			largestDimension = height;

		return largestDimension;
	}

	internal override int CalculateShortestDimension()
	{
		var shortestDimension = length;

		if (width < shortestDimension)
			shortestDimension = width;

		if (height < shortestDimension)
			shortestDimension = height;

		return shortestDimension;
	}

	internal override int CalculateVolume()
	{
		return Length * Width * Height;
	}
}
