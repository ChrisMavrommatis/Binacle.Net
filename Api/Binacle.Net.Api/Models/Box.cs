using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Models
{
    public class Box : IWithID, IWithDimensions<int>, IWithQuantity<int>
    {
        public string ID { get; set; }
        public int Quantity { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
