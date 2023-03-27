using Binacle.Lib.Components.Abstractions.Models;
using Binacle.Lib.Components.Extensions;

namespace Binacle.Api.Models
{
    public class Container : IWithID, IWithDimensions<ushort>
    {
        public Container(string iD)
        {

        }

        public Container(string id, IWithReadOnlyDimensions<ushort> item)
        {
            this.ID = id;
            this.CopyDimensionsFrom(item);
        }

        public string ID { get; set; }
        public ushort Length { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
    }
}
