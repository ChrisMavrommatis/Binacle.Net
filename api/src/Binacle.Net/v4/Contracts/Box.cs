using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("An item to pack or fit, with its dimensions and quantity.")]
public class Box : IWithID, IWithDimensions, IWithQuantity, IIdentifiableItem
{
	[Description(SchemaDescriptions.Id)]
	public required string ID { get; set; } 
	
	[Description(SchemaDescriptions.Quantity)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Quantity { get; set; }
	
	[Description(SchemaDescriptions.Length)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Length { get; set; }
	
	[Description(SchemaDescriptions.Width)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Width { get; set; }
	
	[Description(SchemaDescriptions.Height)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Height { get; set; }
	
	internal static Box From(string id, int length, int width, int height, int quantity)
	{
		return new Box()
		{
			ID = id,
			Length = length,
			Width = width,
			Height = height,
			Quantity = quantity,
		};
	}
}
