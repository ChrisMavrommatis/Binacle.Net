namespace Binacle.Lib.Abstractions.Models
{
    public interface IWithReadOnlyDimensions<T>
        where T : struct
    {
        T Length { get; }
        T Width { get; }
        T Height { get; }
    }
}
