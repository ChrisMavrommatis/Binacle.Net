using Binacle.Lib.Components.Extensions;

namespace Binacle.Lib.Components.Models
{
    public class Bin : VolumetricItem, IWithID
    {
        public Bin(string id, IWithDimensions item) : base(item)
        {
            this.ID = id;
        }

        public string ID { get; set; }

        public static explicit operator Dimensions(Bin bin) => bin.ToDimensions();
    }
}
