using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Binacle.Net.Api.UIModule.ViewModels;
using Binacle.Net.Lib;
using Binacle.PackingVisualizationProtocol;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Pages;

public partial class ProtocolDecoder : ComponentBase
{
	[Inject] 
	protected MessagingService? MessagingService { get; set; }
	
	private Errors errors = new();

	internal ViewModels.ProtocolDecoderViewModel Model { get; set; } = new();
	
	private HashSet<string> encodedResults { get; set; } = new();
	private List<UIModule.Models.DecodedPackingResult> results { get; set; } = new();
	private UIModule.Models.DecodedPackingResult? selectedResult { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
	}

	private bool IsSelected(UIModule.Models.DecodedPackingResult result)
	{
		return this.selectedResult == result;
	}

	private Task DeleteResult(UIModule.Models.DecodedPackingResult result)
	{
		this.results.Remove(result);
		return Task.CompletedTask;
	}
	
	private async Task AddResult()
	{
		var resultString = this.Model.AddResult;

		if (string.IsNullOrWhiteSpace(resultString))
		{
			return;
		}
			
		if(!this.encodedResults.Add(resultString))
		{
			this.errors.Add("Result already added");
			this.Model.AddResult = string.Empty;
			return;
		}
		
		var decodedResult = DecodeResult(resultString);
		if (decodedResult is null)
		{
			this.errors.Add("Could not decode result");
			this.Model.AddResult = string.Empty;
			return;
		}
		
		this.results.Add(decodedResult!);

		this.Model.AddResult = string.Empty;
		
		if(this.results.Count == 1)
		{
			await this.SelectResult(this.results.FirstOrDefault()!);
		}
	}

	private static UIModule.Models.DecodedPackingResult? DecodeResult(string resultString)
	{
		try
		{
			var bytes = Convert.FromBase64String(resultString);
			var (bin, items) =
				PackingVisualizationProtocolSerializer
					.DeserializeInt32<UIModule.Models.Bin, UIModule.Models.PackedItem>(bytes);

			bin.ID = bin.FormatDimensions();
			return new UIModule.Models.DecodedPackingResult()
			{
				Bin = bin,
				PackedItems = items.ToList(),
			};
		}
		catch (Exception ex)
		{
			return null;
		}
	}

	private async Task SelectResult(UIModule.Models.DecodedPackingResult result)
	{
		await this.MessagingService!
			.TriggerAsync<AsyncCallback<(UIModule.Models.Bin?, List<UIModule.Models.PackedItem>?)>>(
				"UpdateScene",
				async () =>
				{
					try
					{
						if (result.Bin is null)
						{
							throw new InvalidOperationException("Selected result has no bin");
						}

						var existingResult = this.results?.FirstOrDefault(x => x.Bin!.ID == result.Bin.ID);
						if (existingResult is null)
						{
							throw new InvalidOperationException("Could not find selected result");
						}

						this.selectedResult = result;
						return (this.selectedResult.Bin, this.selectedResult.PackedItems);
					}
					catch (Exception ex)
					{
						this.errors.Add(ex.Message);
						return (null, null);
					}
				});
	}
}
