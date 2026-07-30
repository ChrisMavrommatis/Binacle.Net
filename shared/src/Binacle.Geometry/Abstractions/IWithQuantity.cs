using System.Numerics;

namespace Binacle.Geometry;

// A thing whose quantity can be set.
public interface IWithQuantity<T> : IWithReadOnlyQuantity<T>
	where T : struct, IBinaryInteger<T>
{
	new T Quantity { get; set; }
}

// Non-generic int shortcut.
public interface IWithQuantity : IWithQuantity<int>
{
}
