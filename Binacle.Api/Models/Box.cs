using Binacle.Lib.Components.Models;

namespace Binacle.Api.Models
{
    public class Box : IWithID, IWithDimensions
    {
        public string ID { get; set; }
        public int Quantity { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
