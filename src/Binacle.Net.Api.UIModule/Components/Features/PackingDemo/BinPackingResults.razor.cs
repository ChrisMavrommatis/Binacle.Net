using Binacle.Net.Api.UIModule.ApiModels;
using Binacle.Net.Api.UIModule.ApiModels.Requests;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPackingResults : ComponentBase
{
	internal List<PackingResult> Results { get; set; }

	protected override void OnInitialized()
	{

		this.Results = new List<PackingResult>
		{
			new PackingResult
			{
				Result =  PackResultType.FullyPacked,
				Bin = new Bin("60x40x10", 60, 40, 10),
				PackedItems = new List<PackedItem>
				{
					new PackedItem("15x10x15-2", 15, 15, 10, 0, 0, 0),
					new PackedItem("15x10x15-2", 15, 15, 10, 15, 0, 0),
					new PackedItem("10x12x15-3", 12, 15, 10, 0, 15, 0),
					new PackedItem("10x12x15-3", 12, 15, 10, 30, 0, 0),
					new PackedItem("10x12x15-3", 12, 15, 10, 15, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 12, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 0, 30, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 42, 0, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 30, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 27, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 15, 30, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 12, 20, 0)
				},
				UnpackedItems = new List<PackedItem>(),
				PackedItemsVolumePercentage = 100,
				PackedBinVolumePercentage = 44.17

			},
			new PackingResult
			{
				Result =  PackResultType.FullyPacked,
				Bin = new Bin("60x40x20", 60, 40, 20),
				PackedItems = new List<PackedItem>
				{
					new PackedItem("15x10x15-2", 15, 15, 10, 0, 0, 0),
					new PackedItem("15x10x15-2", 15, 15, 10, 15, 0, 0),
					new PackedItem("10x12x15-3", 12, 15, 10, 0, 15, 0),
					new PackedItem("10x12x15-3", 12, 15, 10, 30, 0, 0),
					new PackedItem("10x12x15-3", 12, 15, 10, 15, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 12, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 0, 30, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 42, 0, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 30, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 27, 15, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 15, 30, 0),
					new PackedItem("10x2x5-7", 2, 5, 10, 12, 20, 0)
				},
				UnpackedItems = new List<PackedItem>(),
				PackedItemsVolumePercentage = 100,
				PackedBinVolumePercentage = 44.17

			}
		};

	}
}
