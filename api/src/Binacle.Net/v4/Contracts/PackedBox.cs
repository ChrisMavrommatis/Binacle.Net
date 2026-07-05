using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// [Migrate-Review] drop the explicit Binacle.Geometry.IWithDimensions<int>/IWithCoordinates<int> below once the
// lib IWith shims reach the mutable leaf generic (see .agents/plans/shared-geometry-extraction.md).
public class PackedBox :
	IWithID,
	IWithDimensions,
	IWithCoordinates,
	Binacle.Geometry.IWithDimensions<int>,
	Binacle.Geometry.IWithCoordinates<int>
{
	public required string ID { get; set; }
	public required int Length { get; set; }
	public required int Width { get; set; }
	public required int Height { get; set; }
	public required int X { get; set; }
	public required int Y { get; set; }
	public required int Z { get; set; }

	internal static PackedBox From(string id, int length, int width, int height, int x, int y, int z)
	{
		return new PackedBox()
		{
			ID = id,
			Length = length,
			Width = width,
			Height = height,
			X = x,
			Y = y,
			Z = z
		};
	}
	internal static PackedBox From(PackedItem packedItem)
	{
		return new PackedBox()
		{
			ID = packedItem.ID,
			Length = packedItem.Dimensions.Length,
			Width = packedItem.Dimensions.Width,
			Height = packedItem.Dimensions.Height,
			X = packedItem.Coordinates.X,
			Y = packedItem.Coordinates.Y,
			Z = packedItem.Coordinates.Z
		};
	}
}
