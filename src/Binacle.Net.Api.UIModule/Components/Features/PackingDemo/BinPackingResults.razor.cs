using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPackingResults : ComponentBase
{
	[Inject]
	internal Services.PackingDemoState State { get; set; }

	protected override void OnInitialized()
	{
		// register on state.results change to update the state
		base.OnInitialized();
		State.OnResultsChanged += ResultsChanged;
	}

	private Task ResultsChanged(ReadOnlyCollection<Models.PackingResult> results)
	{
		this.StateHasChanged();
		return Task.CompletedTask;
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
		return this.State.IsSelected(result);

	}

	private async Task SelectResultAsync(Models.PackingResult result)
	{
		await this.State.SelectResultAsync(result);
	}

}
