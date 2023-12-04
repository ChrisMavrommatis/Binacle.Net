using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Models
{
    public class Item : BaseItem<int>
    {
        internal Item(string id, IWithReadOnlyDimensions<int> item) : base(id, item)
        {
        }

        internal Item(string id, int length, int width, int height) : base(id, length, width, height)
        {
        }
    }
}
