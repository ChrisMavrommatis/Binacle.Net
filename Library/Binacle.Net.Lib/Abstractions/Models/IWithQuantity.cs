namespace Binacle.Net.Lib.Abstractions.Models
{
    public interface IWithQuantity<T> 
        where T : struct
    {
        T Quantity { get; set; }
    }
}
