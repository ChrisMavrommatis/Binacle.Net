using System.Text.Json;
using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Services;
using Binacle.Net.UIModule.ViewModels;
using Binacle.Lib;
using Binacle.CompactNotation;
using Binacle.ViPaq;
using Microsoft.AspNetCore.Components;
using Bin = Binacle.Net.UIModule.Models.Bin;

namespace Binacle.Net.UIModule.Components.Pages;

public partial class ProtocolDecoder : AppletComponentBase
{
	protected override string Ref => "ProtocolDecoder";
	
	[Inject] 
	internal MessagingService? MessagingService { get; set; }
	
	[Inject] 
	internal LocalStorageService? LocalStorage { get; set; }
	
	private Errors errors = new();

	internal ProtocolDecoderViewModel Model { get; set; } = new();

	private Dictionary<string, DecodedPackingResult> results = new();
	private DecodedPackingResult? selectedResult;

	private const string SavedResultsKey = "ProtocolDecoderSavedResults";

	// Bump when the ViPaq wire changes. "2" is the rebuilt wire (PROTOCOL.md). The stored value carries its own
	// version; anything without a matching version — including the old bare array of tokens — is from a previous
	// format and cannot be decoded, so it is discarded on load.
	private const int CurrentSchemaVersion = 2;

	// The stored shape: the saved tokens plus the schema version that wrote them.
	private sealed record SavedResults(int Version, string[] Results);

	protected override async Task OnAfterRenderAsync(bool isFirstRender)
	{
		if (isFirstRender)
		{
			var savedResults = await this.LoadSavedResultsAsync();
			if (savedResults.Length > 0)
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

	// Reads the stored tokens, but only if they carry the current schema version. Anything else — the old bare
	// array, an older version, or corrupt JSON — is from a previous ViPaq wire and cannot be decoded, so it is
	// discarded and the user is told once.
	private async Task<string[]> LoadSavedResultsAsync()
	{
		try
		{
			var saved = await this.LocalStorage!.GetItemAsync<SavedResults>(SavedResultsKey);
			if (saved is null)
			{
				return [];
			}

			if (saved.Version == CurrentSchemaVersion && saved.Results is not null)
			{
				return saved.Results;
			}
		}
		catch (JsonException)
		{
			// Old bare-array format (or corrupt): fall through and treat as stale.
		}

		this.errors.Add(
			"Your saved results were cleared: the packing token format changed and the old saved tokens can no longer be decoded.");
		await this.SaveResultsAsync();
		return [];
	}

	// Persists the current tokens under the current schema version.
	private async Task SaveResultsAsync()
	{
		await this.LocalStorage!.SetItemAsync(
			SavedResultsKey,
			new SavedResults(CurrentSchemaVersion, this.results.Keys.ToArray()));
	}

	private bool IsSelected(DecodedPackingResult result)
	{
		return this.selectedResult == result;
	}

	private async Task DeleteResult(DecodedPackingResult result)
	{
		this.results.Remove(result.EncodedResult);
		await this.SaveResultsAsync();
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

		await this.SaveResultsAsync();
	}

	private static DecodedPackingResult? DecodeResult(string resultString)
	{
		try
		{
			var (bin, items) =
				ViPaqSerializer.Deserialize<Bin, PackedItem, int>(resultString.FromBase64());

			bin.ID = CompactNotationFormatter.FormatDimensions<int>(bin);
			
			var binVolume = bin.CalculateVolume();
			var itemsVolume = items.Sum(i => i.CalculateVolume());
			var packedBinVolumePercentage = (int)Math.Round(((double)itemsVolume / binVolume) * 100);
			
			return new DecodedPackingResult()
			{
				EncodedResult = resultString,
				Bin = bin,
				PackedItems = items.ToList(),
				PackedBinVolumePercentage = packedBinVolumePercentage
			};
		}
		catch (Exception)
		{
			return null;
		}
	}

	private async Task SelectResult(DecodedPackingResult result)
	{
		await this.MessagingService!
			.TriggerAsync<AsyncCallback<(Bin?, List<PackedItem>?)>>(
				"UpdateScene",
				() =>
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
						var returnedResult = (result.Bin, result.PackedItems);
						return Task.FromResult(returnedResult)!;
					}
					catch (Exception ex)
					{
						this.errors.Add(ex.Message);
						var returnedResult = (default(Bin?), default(List<PackedItem>?));
						return Task.FromResult(returnedResult)!;
					}
				});
	}
}
