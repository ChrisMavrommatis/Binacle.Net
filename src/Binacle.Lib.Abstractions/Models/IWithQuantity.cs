using System.Numerics;

namespace Binacle.Lib.Abstractions.Models;



public interface IWithQuantity : IWithQuantity<int>
{
}

public interface IWithQuantity<T> : IWithReadOnlyQuantity<T>
	where T : INumber<T>
{
	new T Quantity { get; set; }
}
