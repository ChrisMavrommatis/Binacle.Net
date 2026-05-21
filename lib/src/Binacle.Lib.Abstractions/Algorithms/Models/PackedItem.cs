using Binacle.Lib.Models;

namespace Binacle.Lib.Abstractions.Models;

public sealed class PackedItem : ResultItem
{
	internal PackedItem(string id, IWithReadOnlyDimensions dimensions, IWithReadOnlyCoordinates coordinates)
		: this(id, dimensions, new Coordinates(coordinates))
	{
	}
	
	internal PackedItem(string id, IWithReadOnlyDimensions dimensions, Coordinates coordinates)
		: base(id, dimensions)
	{
		this.Coordinates = coordinates;
	}
	
	public Coordinates Coordinates { get; }
}
