using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("A bin (container) to pack items into or fit items against.")]
public class Bin :
	IWithID,
	IWithDimensions,
	IIdentifiableBin
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
	public required  int Height { get; set; }
	
	
	internal static Bin From(string id, int length, int width, int height)
	{
		return new Bin()
		{
			ID = id,
			Length = length,
			Width = width,
			Height = height
		};
	}
	internal static Bin From(PackedBin bin)
	{
		return new Bin()
		{
			ID = bin.ID,
			Height = bin.Height,
			Length = bin.Length,
			Width = bin.Width
		};
	}
}
