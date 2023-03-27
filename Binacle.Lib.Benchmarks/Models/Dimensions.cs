using Binacle.Lib.Components.Abstractions.Models;
using System.Numerics;

namespace Binacle.Lib.Benchmarks.Models
{
    internal class Dimensions<T> : IWithDimensions<T>
       where T : struct, IBinaryInteger<T>
    {
        public Dimensions()
        {

        }

        public Dimensions(T length, T width, T height)
        {
            this.Length = length;
            this.Width = width;
            this.Height = height;
        }
        public T Length { get; set; }
        public T Width { get; set; }
        public T Height { get; set; }
    }
}
