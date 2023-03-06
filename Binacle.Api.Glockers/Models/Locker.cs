using Binacle.Lib.Components.Models;

namespace Binacle.Api.Glockers.Models
{
    public class Locker : IWithDimensions
    {
        public static Locker From(Bin bin)
        {
            return new Locker()
            {
                Size = int.Parse(bin.ID),
                Length = bin.Length,
                Width = bin.Width,
                Height = bin.Height,
            };
        }

        public int Size { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
