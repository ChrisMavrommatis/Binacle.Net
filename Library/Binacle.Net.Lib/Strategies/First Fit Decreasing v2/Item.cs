using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Strategies;

internal sealed partial class FirstFitDecreasing_v2
{
    private sealed class Item : ItemBase
    {
        private readonly int originalLength;
        private readonly int originalWidth;
        private readonly int originalHeight;

        private ushort currentOrientation;
        internal static ushort TotalOrientations = 6;

        internal Item(string id, IWithReadOnlyDimensions<int> item) : base(id, item)
        {
            this.currentOrientation = 0;
            this.originalLength = item.Length;
            this.originalWidth = item.Width;
            this.originalHeight = item.Height;
        }

        internal Item(string id, int length, int width, int height)
            : base(id, length, width, height)
        {
            this.currentOrientation = 0;
            this.originalLength = length;
            this.originalWidth = width;
            this.originalHeight = height;
        }

        internal VolumetricItem Rotate()
        {
            if (this.currentOrientation >= TotalOrientations)
                this.currentOrientation = 0;
            else
                this.currentOrientation++;

            switch (this.currentOrientation)
            {
                // VolumetricItem(item.Length, item.Width, item.Height)
                case 0:
                default:
                    this.length = this.originalLength;
                    this.width = this.originalWidth;
                    this.height = this.originalHeight;
                    break;
                // new VolumetricItem(item.Length, item.Height, item.Width)
                case 1:
                    this.length = this.originalLength;
                    this.width = this.originalHeight;
                    this.height = this.originalWidth;
                    break;
                //new VolumetricItem(item.Width, item.Length, item.Height)
                case 2:
                    this.length = this.originalWidth;
                    this.width = this.originalLength;
                    this.height = this.originalHeight;
                    break;
                // new VolumetricItem(item.Width, item.Height, item.Length)
                case 3:
                    this.length = this.originalWidth;
                    this.width = this.originalHeight;
                    this.height = this.originalLength;
                    break;
                // new VolumetricItem(item.Height, item.Length, item.Width)
                case 4:
                    this.length = this.originalHeight;
                    this.width = this.originalLength;
                    this.height = this.originalWidth;
                    break;
                // new VolumetricItem(item.Height, item.Width, item.Length)
                case 5:
                    this.length = this.originalHeight;
                    this.width = this.originalWidth;
                    this.height = this.originalLength;
                    break;

            }

            return this;
        }
    }
}
