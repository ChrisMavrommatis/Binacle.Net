using System.Numerics;

namespace Binacle.Lib.Abstractions.Models;

public interface IWithReadOnlyQuantity : IWithReadOnlyQuantity<int>
{
}

public interface IWithReadOnlyQuantity<T>
	where T : INumber<T>
{
	T Quantity { get; }
}
