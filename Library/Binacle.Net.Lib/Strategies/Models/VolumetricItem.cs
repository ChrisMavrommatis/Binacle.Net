using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Strategies.Models
{
    internal class VolumetricItem : VolumetricItemBase<ushort, uint>
    {
        internal VolumetricItem(IWithReadOnlyDimensions<ushort> item) : base(item)
        {

        }

        internal VolumetricItem(ushort length, ushort width, ushort height) : base(length, width, height)
        {
        }

        internal override ushort CalculateLongestDimension()
        {
            var largestDimension = length;

            if (width > largestDimension)
                largestDimension = width;

            if (height > largestDimension)
                largestDimension = height;

            return largestDimension;
        }

        internal override ushort CalculateShortestDimension()
        {
            var shortestDimension = length;

            if (width < shortestDimension)
                shortestDimension = width;

            if (height < shortestDimension)
                shortestDimension = height;

            return shortestDimension;
        }

        internal override uint CalculateVolume()
        {
            return (uint)Length * Width * Height;
        }
    }
}
