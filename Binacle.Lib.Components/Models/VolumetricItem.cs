using Binacle.Lib.Components.Exceptions;

namespace Binacle.Lib.Components.Models
{
    public class VolumetricItem 
    {
        private readonly decimal length;
        private readonly decimal width;
        private readonly decimal height;

        private Lazy<decimal> longestDimension;
        private Lazy<decimal> volume;

        public VolumetricItem(VolumetricItem item) : this(item.length, item.width, item.height)
        {
            
        }

        public VolumetricItem(IWithDimensions item) : this(item.Length, item.Width, item.Height)
        {
            
        }

        public VolumetricItem(decimal length, decimal width, decimal height)
        {
            this.length = length;
            this.width = width;
            this.height = height;
            this.volume = new Lazy<decimal>(this.CalculateVolume());
            this.longestDimension = new Lazy<decimal>(this.CalculateLongestDimension());
        }

        public decimal Length { get => this.length; }
        public decimal Width { get => this.width; }
        public decimal Height { get => this.height; }

        public decimal LongestDimension { get => this.longestDimension.Value;  }
        public decimal Volume { get => this.volume.Value; }

        public decimal CalculateVolume()
        {
            return this.length * this.width * this.height;
        }

        public decimal CalculateLongestDimension()
        {
            var largestDimension = this.length;

            if (this.width > largestDimension)
                largestDimension = this.width;

            if (this.height > largestDimension)
                largestDimension = this.height;

            return largestDimension;
        }

        public decimal CalculateShortestDimension()
        {
            var shortestDimension = this.length;

            if (this.width < shortestDimension)
                shortestDimension = this.width;

            if (this.height < shortestDimension)
                shortestDimension = this.height;

            return shortestDimension;
        }
    }
}
