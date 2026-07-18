using Binacle.Lib.Abstractions.Models;
using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("An item that could not be placed in the bin.")]
public class UnpackedBox : IWithID
{
    [Description(SchemaDescriptions.Id)]
    public required string ID { get; set; }
    
    [Description(SchemaDescriptions.Quantity)]
    [OpenApiSchemaRange(Minimum = 1)]
    public required int Quantity { get; set; }

    internal static UnpackedBox From(string id, int quantity)
	{
		return new UnpackedBox()
		{
			ID = id,
			Quantity = quantity
		};
	}
    internal static UnpackedBox From(UnpackedItem unpackedItem)
    {
        return new UnpackedBox()
        {
            ID = unpackedItem.ID,
            Quantity = unpackedItem.Quantity
        };
    }
}
