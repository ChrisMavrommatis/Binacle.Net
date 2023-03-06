using Binacle.Lib.Components.Models;

namespace Binacle.Api.Glockers.Models
{
    public class Item : IWithID, IWithDimensions
    {
        public string ID { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
