using Binacle.Lib.Models;

namespace Binacle.Lib.Abstractions.Models;

public abstract class ResultItem : IWithReadOnlyID, IWithReadOnlyVolume, IWithReadOnlyDimensions
{
	internal ResultItem(string id, IWithReadOnlyDimensions dimensions)
		: this(id, new Dimensions(dimensions))
	{
	}

	internal ResultItem(string id, Dimensions dimensions)
	{
		this.ID = id;
		this.dimensions = dimensions;
		this.Volume = dimensions.CalculateVolume();
	}

	public string ID { get; }
	private Dimensions dimensions;
	public int Volume { get; }

	public int Length => this.dimensions.Length;
	public int Width => this.dimensions.Width;
	public int Height => this.dimensions.Height;
}
