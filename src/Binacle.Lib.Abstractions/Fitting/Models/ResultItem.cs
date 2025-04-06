using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Fitting.Models;

public class ResultItem : IWithID, IWithReadOnlyDimensions
{
	internal ResultItem(string id, IWithReadOnlyDimensions dimensions, int order) :
		this(id, dimensions.Length, dimensions.Width, dimensions.Height, order)
	{

	}
	
	internal ResultItem(string id, int length, int width, int height, int order)
	{
		this.ID = id;
		this.Length = length;
		this.Width = width;
		this.Height = height;
		this.Order = order;
	}

	public string ID { get; set; }

	public int Length { get; }

	public int Width { get; }

	public int Height { get; }
	public int Order { get; }
}
