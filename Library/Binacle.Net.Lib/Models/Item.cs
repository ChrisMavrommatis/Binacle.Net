using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Models
{
    public class Item : BaseItem<ushort>
    {
        public Item(string id, IWithReadOnlyDimensions<ushort> item) : base(id, item)
        {
        }

        public Item(string id, ushort length, ushort width, ushort height) : base(id, length, width, height)
        {
        }
    }
}
