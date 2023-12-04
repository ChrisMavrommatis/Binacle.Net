using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Models
{
    public class Container : IWithID, IWithReadOnlyDimensions<int>
    {
        private readonly IWithReadOnlyDimensions<int> item;

        public Container(string id, IWithReadOnlyDimensions<int> item)
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
