namespace Binacle.Lib.Components.Models
{
    public class Dimensions : IWithDimensions
    {
        public Dimensions()
        {
            
        }

        public Dimensions(decimal length, decimal width, decimal height)
        {
            this.Length = length;
            this.Width = width;
            this.Height = height;
        }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
