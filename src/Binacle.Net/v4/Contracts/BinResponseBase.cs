using Binacle.Lib.Abstractions.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.ViPaq;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591

public abstract class BinResponseBase
{
	public Bin Bin { get; set; } = null!;
	public string AlgorithmUsed { get; set; } = null!;
	public List<PackedBox>? PackedItems { get; set; }
	public List<UnpackedBox>? UnpackedItems { get; set; }
	public decimal PackedItemsVolumePercentage { get; set; }
	public decimal PackedBinVolumePercentage { get; set; }
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
				var serializedResult = ViPaqSerializer.SerializeInt32(result.Bin, result.PackedItems!);
				result.ViPaqData = Convert.ToBase64String(serializedResult);
			}
		}

		return result;
	}
}
