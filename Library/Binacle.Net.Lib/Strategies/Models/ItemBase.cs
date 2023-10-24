using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Strategies.Models
{
    internal abstract class ItemBase : VolumetricItem, IWithID
    {
        internal ItemBase(string id, IWithReadOnlyDimensions<ushort> item) : base(item)
        {
            ID = id;
        }

        internal ItemBase(string id, ushort length, ushort width, ushort height)
            : base(length, width, height)
        {
            ID = id;
        }

        public string ID { get; set; }
    }
}
