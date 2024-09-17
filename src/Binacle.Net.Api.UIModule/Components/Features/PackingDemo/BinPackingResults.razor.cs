using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPackingResults : ComponentBase
{
	[Inject]
	internal Services.PackingDemoState State { get; set; }

	protected override void OnInitialized()
	{
		// register on state.results change to update the state
		base.OnInitialized();
		State.ResultsChanged = new EventCallback(this, UpdateResults);
	}

	private void UpdateResults()
	{
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

	private async Task SelectResultAsync(Models.PackingResult result)
	{

	}

}
