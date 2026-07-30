namespace Binacle.Lib.Abstractions.Models;

// An item with an id, dimensions and a quantity. Read-only — a common view for consumers that
// only read (e.g. the packing log), so a List<concrete item> can be handed off with no copy.
public interface IIdentifiableItem : IWithReadOnlyID, IWithReadOnlyDimensions, IWithReadOnlyQuantity
{
}
