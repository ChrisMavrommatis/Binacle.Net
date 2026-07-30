namespace Binacle.Lib.Abstractions.Models;

public sealed class UnpackedItem : ResultItem, IWithReadOnlyQuantity
{
	internal UnpackedItem(string id, IWithReadOnlyDimensions dimensions, int quantity)
		: base(id, dimensions)
	{
		this.Quantity = quantity;
	}
	
	public int Quantity { get; }
}
