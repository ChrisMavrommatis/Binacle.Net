namespace Binacle.Packing;

public sealed class PackedBin : ResultItem
{
	internal PackedBin(string id, IWithReadOnlyDimensions dimensions)
		: base(id, dimensions)
	{
	}
}
