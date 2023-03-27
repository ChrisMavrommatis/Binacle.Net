using Binacle.Lib.Components.Abstractions.Models;

namespace Binacle.Lib.Models
{
    internal class Item : VolumetricItem, IWithID
    {
        private ushort originalLength;
        private ushort originalWidth;
        private ushort originalHeight;

        private ushort currentOrientation;
        internal static ushort TotalOrientations = 6;

        internal Item(string id, VolumetricItemBase<ushort, uint> item) : base(item)
        {
            this.ID = id;
            this.currentOrientation = 0;
            this.PopulateOriginalDimensions(item.length, item.width, item.height);
        }

        internal Item(string id, IWithReadOnlyDimensions<ushort> item) : base(item)
        {
            this.ID = id;
            this.currentOrientation = 0;
            this.PopulateOriginalDimensions(item.Length, item.Width, item.Height);
        }

        internal Item(string id, ushort length, ushort width, ushort height) : base(length, width, height)
        {
            this.ID = id;
            this.currentOrientation = 0;
            this.PopulateOriginalDimensions(length, width, height);
        }

        public string ID { get; set; }

        private void PopulateOriginalDimensions(ushort length, ushort width, ushort height)
        {
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
