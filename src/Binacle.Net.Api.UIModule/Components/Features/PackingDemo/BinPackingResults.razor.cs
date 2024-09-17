using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPackingResults : ComponentBase
{
	[Inject]
	internal Services.PackingDemoState State { get; set; }

	private string? selectedResult;

	protected override void OnInitialized()
	{
		// register on state.results change to update the state
		base.OnInitialized();
		State.ResultsChanged = new EventCallback(this, UpdateResults);
	}

	private void UpdateResults(List<Models.PackingResult>? results)
	{
		this.selectedResult = results?.FirstOrDefault()?.Bin.ID;
		this.StateHasChanged();
	}

	private string GetColorClass(Models.PackingResult result)
	{
		return result.Result switch
		{
			Models.PackResultType.FullyPacked => "green",
			Models.PackResultType.PartiallyPacked => "orange",
			_ => "red"
		};
	}

	private bool IsSelected(Models.PackingResult result)
	{
		return result.Bin.ID == this.selectedResult;
	}

	private async Task SelectResultAsync(Models.PackingResult result)
	{
		this.selectedResult = result.Bin.ID;
		await this.State.SetResultAsync(result);
	}

}
