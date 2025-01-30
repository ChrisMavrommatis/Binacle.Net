using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Binacle.Net.Api.UIModule.ViewModels;
using Binacle.Net.Lib;
using Binacle.PackingVisualizationProtocol;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Pages;

public partial class ProtocolDecoder : ComponentBase
{
	[Inject] internal MessagingService? MessagingService { get; set; }

	[Inject] internal LocalStorageService? LocalStorage { get; set; }

	private Errors errors = new();

	internal ViewModels.ProtocolDecoderViewModel Model { get; set; } = new();

	private Dictionary<string, Models.DecodedPackingResult> results = new();
	private Models.DecodedPackingResult? selectedResult;

	protected override async Task OnAfterRenderAsync(bool isFirstRender)
	{
		if (isFirstRender)
		{
			var savedResults = await this.LocalStorage!.GetItemAsync<string[]>("ProtocolDecoderSavedResults");
			if (savedResults is not null && savedResults!.Length > 0)
			{
				foreach (var savedResult in savedResults)
				{
					var decodedResult = DecodeResult(savedResult);
					if (decodedResult is not null)
					{
						this.results.Add(savedResult, decodedResult!);
					}
				}

				this.StateHasChanged();
			}
		}

		await base.OnAfterRenderAsync(isFirstRender);
	}

	private bool IsSelected(Models.DecodedPackingResult result)
	{
		return this.selectedResult == result;
	}

	private async Task DeleteResult(Models.DecodedPackingResult result)
	{
		this.results.Remove(result.EncodedResult);
		await this.LocalStorage!.SetItemAsync("ProtocolDecoderSavedResults", this.results.Keys.ToArray());
	}

	private async Task AddResult()
	{
		var resultString = this.Model.AddResult;

		if (string.IsNullOrWhiteSpace(resultString))
		{
			return;
		}

		if (this.results.ContainsKey(resultString))
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

		this.results.Add(resultString, decodedResult!);


		this.Model.AddResult = string.Empty;
		if (this.results.Count == 1)
		{
			await this.SelectResult(this.results.Values.FirstOrDefault()!);
		}

		await this.LocalStorage!.SetItemAsync("ProtocolDecoderSavedResults", this.results.Keys.ToArray());
	}

	private static Models.DecodedPackingResult? DecodeResult(string resultString)
	{
		try
		{
			var bytes = Convert.FromBase64String(resultString);
			var (bin, items) =
				PackingVisualizationProtocolSerializer
					.DeserializeInt32<Models.Bin, Models.PackedItem>(bytes);

			bin.ID = bin.FormatDimensions();
			return new Models.DecodedPackingResult()
			{
				EncodedResult = resultString,
				Bin = bin,
				PackedItems = items.ToList(),
			};
		}
		catch (Exception ex)
		{
			return null;
		}
	}

	private async Task SelectResult(Models.DecodedPackingResult result)
	{
		await this.MessagingService!
			.TriggerAsync<AsyncCallback<(Models.Bin?, List<Models.PackedItem>?)>>(
				"UpdateScene",
				async () =>
				{
					try
					{
						if (result.Bin is null)
						{
							throw new InvalidOperationException("Selected result has no bin");
						}

						if (!(this.results?.TryGetValue(result.EncodedResult, out var existingResult) ?? false))
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
