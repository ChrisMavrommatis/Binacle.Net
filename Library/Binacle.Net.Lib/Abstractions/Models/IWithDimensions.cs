namespace Binacle.Net.Lib.Abstractions.Models
{
    public interface IWithDimensions<T> : IWithReadOnlyDimensions<T>
         where T : struct
    {
        new T Length { get; set; }
        new T Width { get; set; }
        new T Height { get; set; }
    }
}
