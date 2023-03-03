using Binacle.Lib.Components.Extensions;

namespace Binacle.Lib.Components.Models
{
    public class Item : VolumetricItem, IWithID
    {
        public Item(string id, IWithDimensions item) : base(item)
        {
            this.ID = id;
        }

        public string ID { get; set; }
    }
}
