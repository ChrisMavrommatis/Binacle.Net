using Binacle.Api.Components.Models;

namespace Binacle.Api.BoxNow.Models
{
    public class BoxItem: IWithID, IWithDimensions
    {
        public string ID { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
