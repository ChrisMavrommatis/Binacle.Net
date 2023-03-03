namespace Binacle.Lib.Components.Models
{
    public class Bin : VolumetricItem, IWithID
    {
        public Bin(string id, IWithDimensions item) : base(item)
        {
            this.ID = id;
        }

        public string ID { get; set; }
    }
}
