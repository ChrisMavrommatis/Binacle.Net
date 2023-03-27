using Binacle.Lib.Components.Abstractions.Models;

namespace Binacle.Api.Glockers.Models
{
    public class BoxItem : IWithID
    {
        public string ID { get; set; }
        public int Quantity { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
