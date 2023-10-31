using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Models
{
    public class Container : IWithID, IWithDimensions<ushort>
    {
        public Container(string iD)
        {

        }

        public Container(string id, IWithReadOnlyDimensions<ushort> item)
        {
            this.ID = id;
            this.Length = item.Length;
            this.Width = item.Width;
            this.Height = item.Height;
        }

        public string ID { get; set; }
        public ushort Length { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
    }
}
