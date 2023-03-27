using Binacle.Lib.Components.Abstractions.Models;
using System.Numerics;

namespace Binacle.Lib.Models
{
    internal abstract class VolumetricItemBase<TDimensions, TVolume> : IWithReadOnlyDimensions<TDimensions>, IWithReadOnlyVolume<TVolume>
        where TDimensions : struct, IBinaryInteger<TDimensions>
        where TVolume : struct, IBinaryInteger<TVolume>
    {
        //protected internal readonly TDimensions originalLength;
        //protected internal readonly TDimensions originalWidth;
        //protected internal readonly TDimensions originalHeight;

        protected internal TDimensions length;
        protected internal TDimensions width;
        protected internal TDimensions height;

        //private ushort currentOrientation;
        //private ushort totalOrientations;

        private TDimensions longestDimension;
        private TVolume volume;

        internal VolumetricItemBase(VolumetricItemBase<TDimensions, TVolume> item) : this(item.length, item.width, item.height)
        {

        }

        internal VolumetricItemBase(IWithReadOnlyDimensions<TDimensions> item) : this(item.Length, item.Width, item.Height)
        {

        }

        internal VolumetricItemBase(TDimensions length, TDimensions width, TDimensions height)
        {
            this.length = length;
            this.width = width;
            this.height = height;

            this.volume = this.CalculateVolume();
            this.longestDimension = this.CalculateLongestDimension();
        }

        public TDimensions Length { get => this.length; }
        public TDimensions Width { get => this.width; }
        public TDimensions Height { get => this.height; }

        internal TDimensions LongestDimension { get => this.longestDimension; }
        public TVolume Volume { get => this.volume; }

        internal abstract TVolume CalculateVolume();
        internal abstract TDimensions CalculateLongestDimension();
        internal abstract TDimensions CalculateShortestDimension();


    }
}
