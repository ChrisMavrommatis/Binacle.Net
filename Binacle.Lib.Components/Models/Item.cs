using Binacle.Lib.Components.Abstractions.Models;

namespace Binacle.Lib.Components.Models
{
    public class Item : BaseItem<ushort, uint>
    {
        public Item(string id, IWithReadOnlyDimensions<ushort> item) : base(id, item)
        {
                
        }

        public Item(string id, ushort length, ushort width, ushort height) : base(id, length, width, height)
        {
                
        }
    }
}