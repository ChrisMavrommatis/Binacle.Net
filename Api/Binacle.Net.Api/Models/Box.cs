using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Models
{
    public class Box : IWithID, IWithDimensions<ushort>
    {
        public string ID { get; set; }
        public int Quantity { get; set; }
        public ushort Length { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
    }
}
