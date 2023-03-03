using Binacle.Lib.Components.Models;

namespace Binacle.Api.BoxNow.Models
{
    public class LockerBin : IWithDimensions
    {
        public static LockerBin From(Bin bin)
        {
            return new LockerBin()
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
