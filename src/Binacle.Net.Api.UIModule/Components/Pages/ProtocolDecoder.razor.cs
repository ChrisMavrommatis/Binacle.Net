using Binacle.Net.Api.UIModule.Models;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Pages;

public partial class ProtocolDecoder : ComponentBase
{
	private List<UIModule.Models.DecodedPackingResult>? results { get; set; } = new();
	private UIModule.Models.DecodedPackingResult? selectedResult { get; set; }
	protected override void OnInitialized()
	{
		this.results =
		[
			new UIModule.Models.DecodedPackingResult()
			{
				Bin = new Bin()
				{
					ID ="60x40x30",
					Length = 60,
					Width = 40,
					Height = 30,
				},
				PackedItems = new()
				{
					new PackedItem()
					{
						ID = "1",
						Dimensions = new Dimensions()
						{
							Length = 12,
							Width = 15,
							Height = 10,
						},
						Coordinates = new Coordinates()
						{
							X = 0,
							Y = 0,
							Z = 0,
						}
					},
					new PackedItem()
					{
						ID = "2",
						Dimensions = new Dimensions()
						{
							Length = 12,
							Width = 10,
							Height = 15,
						},
						Coordinates = new Coordinates()
						{
							X = 12,
							Y = 0,
							Z = 0,
						}
					},
					new PackedItem()
					{
						ID = "3",
						Dimensions = new Dimensions()
						{
							Length = 2,
							Width = 5,
							Height = 10,
						},
						Coordinates = new Coordinates()
						{
							X = 0,
							Y = 15,
							Z = 0,
						}
					},
					new PackedItem()
					{
						ID = "4",
						Dimensions = new Dimensions()
						{
							Length = 2,
							Width = 5,
							Height = 10,
						},
						Coordinates = new Coordinates()
						{
							X = 0,
							Y = 0,
							Z = 10,
						}
					},
					
				},
			},
			new UIModule.Models.DecodedPackingResult()
			{
				Bin = new Bin()
				{
					ID ="60x40x30",
					Length = 60,
					Width = 40,
					Height = 30,
				},
				PackedItems = new()
				{
					new PackedItem()
					{
						ID = "1",
						Dimensions = new Dimensions()
						{
							Length = 12,
							Width = 15,
							Height = 10,
						},
						Coordinates = new Coordinates()
						{
							X = 0,
							Y = 0,
							Z = 0,
						}
					},
					new PackedItem()
					{
						ID = "2",
						Dimensions = new Dimensions()
						{
							Length = 12,
							Width = 10,
							Height = 15,
						},
						Coordinates = new Coordinates()
						{
							X = 12,
							Y = 0,
							Z = 0,
						}
					},
					new PackedItem()
					{
						ID = "3",
						Dimensions = new Dimensions()
						{
							Length = 2,
							Width = 5,
							Height = 10,
						},
						Coordinates = new Coordinates()
						{
							X = 0,
							Y = 15,
							Z = 0,
						}
					},
					new PackedItem()
					{
						ID = "4",
						Dimensions = new Dimensions()
						{
							Length = 2,
							Width = 5,
							Height = 10,
						},
						Coordinates = new Coordinates()
						{
							X = 0,
							Y = 0,
							Z = 10,
						}
					},
					
				},
				
			},
		];
		this.selectedResult = this.results.FirstOrDefault();
		base.OnInitialized();
	}
	
	private bool IsSelected(UIModule.Models.DecodedPackingResult result)
	{
		return this.selectedResult == result;
	}
}

