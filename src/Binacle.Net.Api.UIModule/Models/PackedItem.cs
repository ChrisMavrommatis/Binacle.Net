using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.Models;

internal class PackedItem : IWithID
{
	public PackedItem()
	{
		this.ID = string.Empty;
	}

	public PackedItem(string id, int length, int width, int height, int x, int y, int z)
	{
		this.ID = id;
		this.Dimensions = new Dimensions(length, width, height);
		this.Coordinates = new Coordinates(x, y, z);
	}

	public required string ID { get; set; }

	public Dimensions Dimensions { get; set; }
	public Coordinates? Coordinates { get; set; }

}
