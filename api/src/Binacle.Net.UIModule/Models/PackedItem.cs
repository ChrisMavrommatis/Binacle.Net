using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.UIModule.Models;

// [Migrate-Review] drop the explicit Binacle.Geometry.IWithDimensions<int>/IWithCoordinates<int> below once the
// lib IWith shims reach the mutable leaf generic (see .agents/plans/shared-geometry-extraction.md).
internal class PackedItem :
	IWithID,
	IWithDimensions,
	Binacle.Geometry.IWithDimensions<int>,
	Binacle.Geometry.IWithCoordinates<int>
{
	// Json Deserialize
	public PackedItem()
	{
		this.ID = string.Empty;
	}
	
	public PackedItem(string id, int length, int width, int height, int x, int y, int z)
	{
		this.ID = id;
		this.Length = length;
		this.Width = width;
		this.Height = height;
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public string ID { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
}
