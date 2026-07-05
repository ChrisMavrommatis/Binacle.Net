namespace Binacle.Lib.Abstractions.Models;

// Non-generic int shortcut over the shared Binacle.Geometry generic interface.
// Base is fully qualified so it is never mistaken for this same-named non-generic interface.
public interface IWithReadOnlyCoordinates : Binacle.Geometry.IWithReadOnlyCoordinates<int>
{
}
