using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.Models;

internal class Bin : IWithID, IWithReadOnlyDimensions
{
	public Bin()
	{
		
	}
	public Bin(string id, IWithReadOnlyDimensions item)
		: this(id, item.Length, item.Width, item.Height)
	{

	}

	public Bin(string id, int length, int width, int height)
	{
		ID = id;
		Length = length;
		Width = width;
		Height = height;
	}

	public string ID { get; set; }
	public int Length { get; }

	public int Width { get; }

	public int Height { get; }
}
