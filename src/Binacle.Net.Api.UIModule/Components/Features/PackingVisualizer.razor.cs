using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class PackingVisualizer : ComponentBase
{
	[Inject]
	protected IJSRuntime JS { get; set; }
	
	[Inject]
    protected MessagingService? MessagingService { get; set; }

    [Parameter]
	public UIModule.Models.Bin? Bin { get; set; }

	private List<UIModule.Models.PackedItem>? items;

	private Dictionary<string, Models.Control> controls = [];

	protected override void OnInitialized()
	{
		this.controls.Add("first", new Models.Control("control-first", "first_page", FirstAsync));
		this.controls.Add("previous", new Models.Control("control-previous", "chevron_left", PreviousAsync));
		this.controls.Add("repeat", new Models.Control("control-repeat", "repeat_one", RepeatAsync));
		this.controls.Add("next", new Models.Control("control-next", "chevron_right", NextAsync));
		this.controls.Add("last", new Models.Control("control-last", "last_page", LastAsync));
		
		this.MessagingService?.On<AsyncCallback<(UIModule.Models.Bin?, List<UIModule.Models.PackedItem>?)>>("UpdateScene", UpdateSceneAsync);

		base.OnInitialized();
	}


	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if(firstRender)
		{
			await JS.InvokeVoidAsync("binacle.initialize", this.Bin);
		}
		await base.OnAfterRenderAsync(firstRender);
	}

	internal async Task UpdateSceneAsync(
		AsyncCallback<(UIModule.Models.Bin?, List<UIModule.Models.PackedItem>?)> getUpdate
		)
	{
		await JS.InvokeVoidAsync("binacle.loadingStart");
		var (bin, items) = await getUpdate();

		if (bin is not null && items is not null)
		{
			this.Bin = bin;
			this.items = items;

			await JS.InvokeVoidAsync("binacle.redrawScene", this.Bin, this.items);
		}

		await JS.InvokeVoidAsync("binacle.loadingEnd");
	}

	private void DisableAllControls()
	{
		foreach (var control in this.controls.Values)
		{
			control.IsEnabled = false;
		}
	}

	
	private async Task FirstAsync()
	{
	}

	private async Task PreviousAsync()
	{
	}

	private async Task RepeatAsync()
	{
	}

	private async Task NextAsync()
	{
	}

	private async Task LastAsync()
	{
	}
}
