using Binacle.Lib.Components.Abstractions.Models;

namespace Binacle.Lib.Models
{
    internal class Bin : VolumetricItem, IWithID
    {
        internal Bin(string id, VolumetricItemBase<ushort, uint> item) : base(item)
        {
            this.ID = id;
        }

        internal Bin(string id, IWithReadOnlyDimensions<ushort> item) : base(item)
        {
            this.ID = id;
        }

        internal Bin(string id, ushort length, ushort width, ushort height) : base(length, width, height)
        {
            this.ID = id;
        }

        public string ID { get; set; }
    }
}
