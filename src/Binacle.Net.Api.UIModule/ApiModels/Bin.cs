using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.ApiModels;

internal class Bin : IWithID, IWithReadOnlyDimensions
{
	public Bin(IWithReadOnlyDimensions item)
		: this(item.Length, item.Width, item.Height)
	{

	}

	public Bin(int length, int width, int height)
	{
		this.ID = $"{length}x{width}x{height}";
		this.Length = length;
		this.Width = width;
		this.Height = height;
	}

	public string ID { get; set; }
	public int Length { get; }

	public int Width { get; }

	public int Height { get; }
}
