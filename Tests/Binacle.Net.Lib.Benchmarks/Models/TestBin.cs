using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Benchmarks.Models
{
    internal class TestBin : IItemWithReadOnlyDimensions<int>
    {
        private readonly IWithReadOnlyDimensions<int> item;

        public TestBin(string id, IWithReadOnlyDimensions<int> item)
        {
            this.ID = id;
            this.item = item;
        }
        public string ID { get; set; }

        public int Length { get => this.item.Length; }
        public int Width { get => this.item.Width; }
        public int Height { get => this.item.Height; }
    }
}
