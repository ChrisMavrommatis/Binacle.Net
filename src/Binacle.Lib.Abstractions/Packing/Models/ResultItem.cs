using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Models;

namespace Binacle.Lib.Packing.Models;

public class ResultItem : IWithID, IWithReadOnlyVolume
{
	internal ResultItem(string id, IWithReadOnlyDimensions dimensions) :
		this(id, new Dimensions(dimensions))
	{

	}
	internal ResultItem(string id, IWithReadOnlyDimensions dimensions, IWithReadOnlyCoordinates coordinates) :
		this(id, new Dimensions(dimensions), new Coordinates(coordinates))
	{

	}
	internal ResultItem(string id, Dimensions dimensions, Coordinates? coordinates = null)
	{
		ID = id;
		Dimensions = dimensions;
		Coordinates = coordinates;
		Volume = dimensions.CalculateVolume();
	}

	public string ID { get; set; }
	public Dimensions Dimensions { get; set; }
	public Coordinates? Coordinates { get; set; }
	public int Volume { get; }
}
