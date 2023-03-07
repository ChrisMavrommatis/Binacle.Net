namespace Binacle.Lib.Components.Models
{
    public interface IWithDimensions
    {
        decimal Length { get; set; }
        decimal Width { get; set; }
        decimal Height { get; set; }
    }
}
