using Binacle.Lib.Components.Abstractions.Models;

namespace Binacle.Lib.Tests.Models
{
    public class TestItemWithDimensions : IWithID, IWithDimensions<ushort>
    {
        public string ID { get; set; }
        public ushort Length { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
    }

}
