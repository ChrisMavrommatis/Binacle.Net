using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Tests.Models
{
    public class TestItemWithDimensions : IWithID, IWithDimensions<int>
    {
        public string ID { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
