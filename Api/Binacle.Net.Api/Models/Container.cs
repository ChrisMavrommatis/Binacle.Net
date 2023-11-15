using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.Models
{
    public class Container : IWithID, IWithDimensions<int>
    {
        public Container()
        {

        }

        public Container(string id, int length, int width, int height)
        {
            this.ID = id;
            this.Length = length;
            this.Width = width;
            this.Height = height;
        }

        public string ID { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
