namespace Binacle.Packing;

// A bin with an id and dimensions. Read-only — a common view for consumers that only read
// (e.g. the packing log), so a List<concrete bin> can be handed off with no copy.
public interface IIdentifiableBin : IWithReadOnlyID, IWithReadOnlyDimensions
{
}
