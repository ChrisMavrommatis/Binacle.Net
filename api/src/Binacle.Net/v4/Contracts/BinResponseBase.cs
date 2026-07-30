using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.ViPaq;
using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591

[OpenApiRequireNonNullable]
public abstract class BinResponseBase
{
	[Description(SchemaDescriptions.Bin)]
	public Bin Bin { get; set; } = null!;
	
	[Description(SchemaDescriptions.AlgorithmUsed)]
	public string AlgorithmUsed { get; set; } = null!;
	
	[Description(SchemaDescriptions.PackedItems)]
	public List<PackedBox>? PackedItems { get; set; }
	
	[Description(SchemaDescriptions.UnpackedItems)]
	public List<UnpackedBox>? UnpackedItems { get; set; }
	
	[Description(SchemaDescriptions.PackedItemsVolumePercentage)]
	[OpenApiSchemaRange(Minimum = 0, Maximum = 100)]
	public decimal PackedItemsVolumePercentage { get; set; }
	
	[Description(SchemaDescriptions.PackedBinVolumePercentage)]
	[OpenApiSchemaRange(Minimum = 0, Maximum = 100)]
	public decimal PackedBinVolumePercentage { get; set; }
	
	[Description(SchemaDescriptions.ViPaqData)]
	public string? ViPaqData { get; set; }

	protected static T From<T>(
		OperationParameters parameters,
		OperationResult operationResult
	)
		where T : BinResponseBase, new()
	{
		var result = new T()
		{
			Bin = Bin.From(operationResult.Bin),
			AlgorithmUsed = operationResult.AlgorithmInfo.Algorithm.ToFastString(),
			PackedItems = operationResult.PackedItems
				.Select(x => PackedBox.From(x))
				.ToList(),
			UnpackedItems = operationResult.UnpackedItems
				.Select(x => UnpackedBox.From(x))
				.ToList(),
			PackedBinVolumePercentage = operationResult.PackedBinVolumePercentage,
			PackedItemsVolumePercentage = operationResult.PackedItemsVolumePercentage,
		};
		if (parameters.IncludeViPaqData)
		{
			if (result.PackedItems is not null && result.PackedItems.Count > 0)
			{
				result.ViPaqData = ViPaqSerializer
					.Serialize<Bin, PackedBox, int>(result.Bin, result.PackedItems!)
					.ToBase64();
			}
		}

		return result;
	}
}
