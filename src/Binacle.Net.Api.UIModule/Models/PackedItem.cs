using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.Models;

internal class PackedItem : IWithID
{
	public PackedItem()
	{

	}
	public PackedItem(string id, int length, int width, int height, int x, int y, int z)
	{
		ID = id;
		Dimensions = new Dimensions(length, width, height);
		Coordinates = new Coordinates(x, y, z);

	}

	public string ID { get; set; } = string.Empty;

	public Dimensions Dimensions { get; set; }
	public Coordinates? Coordinates { get; set; }

}
