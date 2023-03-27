using Binacle.Lib.Components.Abstractions.Models;

namespace Binacle.Lib.Models
{
    internal class VolumetricItem : VolumetricItemBase<ushort, uint>
    {
        internal VolumetricItem(VolumetricItemBase<ushort, uint> item) : base(item)
        {

        }

        internal VolumetricItem(IWithReadOnlyDimensions<ushort> item) : base(item)
        {

        }

        internal VolumetricItem(ushort length, ushort width, ushort height) : base(length, width, height)
        {
        }

        internal override ushort CalculateLongestDimension()
        {
            var largestDimension = this.length;

            if (this.width > largestDimension)
                largestDimension = this.width;

            if (this.height > largestDimension)
                largestDimension = this.height;

            return largestDimension;
        }

        internal override ushort CalculateShortestDimension()
        {
            var shortestDimension = this.length;

            if (this.width < shortestDimension)
                shortestDimension = this.width;

            if (this.height < shortestDimension)
                shortestDimension = this.height;

            return shortestDimension;
        }

        internal override uint CalculateVolume()
        {
            return (uint)this.Length * this.Width * this.Height;
        }
    }
}
