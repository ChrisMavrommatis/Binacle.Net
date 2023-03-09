using Binacle.Lib.Components.Extensions;
using Binacle.Lib.Components.Models;

namespace Binacle.Api.Models
{
    public class Container : IWithID, IWithDimensions
    {
        public Container()
        {

        }

        public Container(string id, IWithDimensions item)
        {
            this.ID = id;
            this.CopyDimensionsFrom(item);
        }

        public string ID { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
