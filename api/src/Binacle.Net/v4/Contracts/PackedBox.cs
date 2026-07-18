using Binacle.Lib.Abstractions.Models;
using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("An item placed inside the bin, with its position.")]
public class PackedBox :
	IWithID,
	IWithDimensions,
	IWithCoordinates
{
	[Description(SchemaDescriptions.Id)]
	public required string ID { get; set; }
	
	[Description(SchemaDescriptions.Length)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Length { get; set; }
	
	[Description(SchemaDescriptions.Width)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Width { get; set; }
	
	[Description(SchemaDescriptions.Height)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Height { get; set; }
	
	[Description(SchemaDescriptions.CoordinateX)]
	[OpenApiSchemaRange(Minimum = 0)]
	public required int X { get; set; }
	
	[Description(SchemaDescriptions.CoordinateY)]
	[OpenApiSchemaRange(Minimum = 0)]
	public required int Y { get; set; }
	
	[Description(SchemaDescriptions.CoordinateZ)]
	[OpenApiSchemaRange(Minimum = 0)]
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
			Length = packedItem.Length,
			Width = packedItem.Width,
			Height = packedItem.Height,
			X = packedItem.X,
			Y = packedItem.Y,
			Z = packedItem.Z
		};
	}
}
