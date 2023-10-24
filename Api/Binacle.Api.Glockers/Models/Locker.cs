namespace Binacle.Api.Glockers.Models
{
    public class Locker 
    {
        public Locker(int size, Lib.Components.Models.Item item)
        {
            this.Size = size;
            this.Length = item.Length;
            this.Width = item.Width;
            this.Height = item.Height;
        }
        public int Size { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }
}
