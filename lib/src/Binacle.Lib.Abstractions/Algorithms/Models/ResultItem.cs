using Binacle.Lib.Models;

namespace Binacle.Lib.Abstractions.Models;

public abstract class ResultItem : IWithReadOnlyID, IWithReadOnlyVolume
{
	internal ResultItem(string id, IWithReadOnlyDimensions dimensions)
		: this(id, new Dimensions(dimensions))
	{
	}

	internal ResultItem(string id, Dimensions dimensions)
	{
		this.ID = id;
		this.Dimensions = dimensions;
		this.Volume = dimensions.CalculateVolume();
	}

	public string ID { get; }
	public Dimensions Dimensions { get; }
	public int Volume { get; }
}
