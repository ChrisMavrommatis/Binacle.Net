using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Binacle.Net.Lib.Abstractions.Models;
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
	public UIModule.Models.Bin? InitialBin { get; set; }
	
	private UIModule.Models.Bin? bin { get; set; }

	private List<UIModule.Models.PackedItem>? items;

	private Dictionary<string, Models.Control> controls = [];
	
	private int itemsRendered;
	private CancellationTokenSource? cancellationTokenSource;
	private bool repeating;

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

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if(firstRender)
		{
			await this.InitializeAsync(this.InitialBin); 
		}
		await base.OnAfterRenderAsync(firstRender);
	}

	private async Task InitializeAsync(UIModule.Models.Bin? bin)
	{
		await JS.InvokeVoidAsync("binacle.initialize", bin);
	}

	private async Task UpdateSceneAsync(
		AsyncCallback<(UIModule.Models.Bin?, List<UIModule.Models.PackedItem>?)> getUpdate
		)
	{
		await this.UpdateLoadingStartAsync();
		this.DisableAllControls();
		var (bin, items) = await getUpdate();

		if (bin is not null && items is not null)
		{
			this.bin = bin;
			this.items = items;

			await this.RedrawSceneAsync(this.bin, this.items);
		}

		this.itemsRendered = this.items.Count;
		this.UpdateControlsStatus();

		this.StateHasChanged();
		await this.UpdateLoadingEndAsync();
	}
	
	private void UpdateControlsStatus()
	{
		if (this.bin is null || this.items is null || !this.items.Any())
		{
			this.DisableAllControls();
			return;
		}

		this.controls["repeat"].IsEnabled = true;

		if (this.itemsRendered <= 0)
		{
			this.controls["first"].IsEnabled = false;
			this.controls["previous"].IsEnabled = false;
			this.controls["next"].IsEnabled = true;
			this.controls["last"].IsEnabled = true;
			return;
		}

		if (this.itemsRendered >= this.items!.Count)
		{
			this.controls["first"].IsEnabled = true;
			this.controls["previous"].IsEnabled = true;
			this.controls["next"].IsEnabled = false;
			this.controls["last"].IsEnabled = false;
			return;

		}

		this.controls["first"].IsEnabled = true;
		this.controls["previous"].IsEnabled = true;
		this.controls["repeat"].IsEnabled = true;
		this.controls["next"].IsEnabled = true;
		this.controls["last"].IsEnabled = true;
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
		this.DisableAllControls();
		this.StateHasChanged();
		await this.RedrawSceneAsync(this.bin!, null);
		this.itemsRendered = 0;
		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task PreviousAsync()
	{
		this.DisableAllControls();
		this.StateHasChanged();

		var index = this.itemsRendered - 1;

		if (index > -1)
		{
			await this.RemoveItemFromSceneAsync(index);
			this.itemsRendered -= 1;
		}
	
		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task RepeatAsync()
	{
		if (this.repeating)
		{
			await this.cancellationTokenSource!.CancelAsync();
			this.repeating = false;
			this.controls["repeat"].Icon = "repeat_one";
			this.UpdateControlsStatus();
			this.StateHasChanged();
			return;
		}

		this.controls["first"].IsEnabled = false;
		this.controls["previous"].IsEnabled = false;
		this.controls["next"].IsEnabled = false;
		this.controls["last"].IsEnabled = false;

		this.repeating = true;
		this.controls["repeat"].Icon = "cancel";
		this.StateHasChanged();

		this.itemsRendered = 0;
		this.cancellationTokenSource = new CancellationTokenSource();

		await this.RedrawSceneAsync(this.bin!, null);

		await Task.Run(async () =>
		{
			if (!this.cancellationTokenSource.Token.IsCancellationRequested)
			{
				for (var i = 0; i < this.items!.Count; i++)
				{
					var item = this.items![i];
					await this.AddItemToSceneAsync(this.bin, item, i);
					this.itemsRendered += 1;
					await Task.Delay(1000, this.cancellationTokenSource.Token);

				}
			}
		}, this.cancellationTokenSource.Token);


		this.repeating = false;
		this.controls["repeat"].Icon = "repeat_one";
		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task NextAsync()
	{
		this.DisableAllControls();
		this.StateHasChanged();

		var index = this.itemsRendered;

		if (index < this.items!.Count)
		{
			var item = this.items![index];
			await this.AddItemToSceneAsync(bin, item, index);
			this.itemsRendered += 1;
		}

		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task LastAsync()
	{
		this.DisableAllControls();
		this.StateHasChanged();
		await this.RedrawSceneAsync(this.bin!, this.items!);
		this.itemsRendered = this.items!.Count;
		this.UpdateControlsStatus();
		this.StateHasChanged();
	}
	
	private ValueTask UpdateLoadingStartAsync()
		=> this.JS.InvokeVoidAsync("binacle.loadingStart");

	private ValueTask UpdateLoadingEndAsync()
		=> this.JS.InvokeVoidAsync("binacle.loadingEnd");
	
	private ValueTask RedrawSceneAsync<TBin>(TBin bin, List<UIModule.Models.PackedItem>? items)
		where TBin : IWithID, IWithReadOnlyDimensions
		=> this.JS.InvokeVoidAsync("binacle.redrawScene", bin, items);

	private ValueTask AddItemToSceneAsync<TBin>(TBin bin, UIModule.Models.PackedItem item, int index)
		=> this.JS.InvokeVoidAsync("binacle.addItemToScene", bin, item, index);

	private ValueTask RemoveItemFromSceneAsync(int index)
		=> this.JS.InvokeVoidAsync("binacle.removeItemFromScene", index);

}
