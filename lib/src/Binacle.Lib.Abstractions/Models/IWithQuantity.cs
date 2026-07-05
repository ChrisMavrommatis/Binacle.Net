namespace Binacle.Lib.Abstractions.Models;

// Non-generic int shortcut over the shared Binacle.Geometry generic interface (settable Quantity).
// Base is fully qualified so it is never mistaken for this same-named non-generic interface.
public interface IWithQuantity : Binacle.Geometry.IWithQuantity<int>
{
}
