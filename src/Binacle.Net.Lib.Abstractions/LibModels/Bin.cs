using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Models;

public class Bin : IWithID, IWithReadOnlyDimensions
{
	public Bin(string id, IWithReadOnlyDimensions item): 
		this(id, item.Length, item.Width, item.Height)
	{

	}
	public Bin(string id, int length, int width, int height)
	{
		this.ID = id;
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}

	public string ID { get; set; }
	public int Length { get; }
	public int Width { get; }
	public int Height { get; }
}
