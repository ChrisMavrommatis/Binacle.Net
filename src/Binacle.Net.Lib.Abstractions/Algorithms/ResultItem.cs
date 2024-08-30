using Binacle.Net.Lib.Abstractions.ExtensionMethods;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Lib;

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
		this.ID = id;
		this.Dimensions = dimensions;
		this.Coordinates = coordinates;
		this.Volume = dimensions.CalculateVolume();
	}

	public string ID { get; set; }
	public Dimensions Dimensions { get; set; }
	public Coordinates? Coordinates { get; set; }
	public int Volume { get; }
}
