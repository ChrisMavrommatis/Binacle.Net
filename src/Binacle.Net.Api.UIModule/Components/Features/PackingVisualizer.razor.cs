using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class PackingVisualizer : ComponentBase
{
	[Inject]
	internal MessagingService? MessagingService { get; set; }
	
	[Inject]
	internal BinacleVisualizerService? Binacle { get; set; }

	[Parameter] 
	public Bin? InitialBin { get; set; }

	private Bin? bin;

	private List<PackedItem>? items;

	private Dictionary<string, Control> controls = [];

	private int itemsRendered;
	private CancellationTokenSource? cancellationTokenSource;
	private bool repeating;

	protected override void OnInitialized()
	{
		this.controls.Add("first", new Control("control-first", "first_page", FirstAsync));
		this.controls.Add("previous", new Control("control-previous", "chevron_left", PreviousAsync));
		this.controls.Add("repeat", new Control("control-repeat", "repeat_one", RepeatAsync));
		this.controls.Add("next", new Control("control-next", "chevron_right", NextAsync));
		this.controls.Add("last", new Control("control-last", "last_page", LastAsync));

		this.MessagingService?.On<AsyncCallback<(Bin?, List<PackedItem>?)>>(
			"UpdateScene", UpdateSceneAsync);
		base.OnInitialized();
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await this.Binacle!.InitializeSceneAsync(this.InitialBin!);
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	private async Task UpdateSceneAsync(
		AsyncCallback<(Bin?, List<PackedItem>?)> getUpdate
	)
	{
		await this.StopRepeatingAsync();
		await this.Binacle!.UpdateLoadingStartAsync();
		this.DisableAllControls();
		var (bin, items) = await getUpdate();

		if (bin is not null && items is not null)
		{
			this.bin = bin;
			this.items = items;

			await this.Binacle.RedrawSceneAsync(this.bin, this.items);
		}

		this.itemsRendered = this.items!.Count;
		this.UpdateControlsStatus();

		this.StateHasChanged();
		await this.Binacle.UpdateLoadingEndAsync();
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
		await this.Binacle!.RedrawSceneAsync(this.bin!, null);
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
			await this.Binacle!.RemoveItemFromSceneAsync(index);
			this.itemsRendered -= 1;
		}

		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task RepeatAsync()
	{
		if (this.repeating)
		{
			await this.StopRepeatingAsync();
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

		await this.Binacle!.RedrawSceneAsync(this.bin!, null);

		await Task.Run(async () =>
		{
			if (!this.cancellationTokenSource.Token.IsCancellationRequested)
			{
				for (var i = 0; i < this.items!.Count; i++)
				{
					var item = this.items![i];
					await this.Binacle.AddItemToSceneAsync(this.bin, item, i);
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

	private async Task StopRepeatingAsync()
	{
		if (this.cancellationTokenSource is not null)
		{
			await this.cancellationTokenSource!.CancelAsync();
		}

		this.repeating = false;
		this.controls["repeat"].Icon = "repeat_one";
		this.UpdateControlsStatus();
		this.StateHasChanged();
		return;
	}

	private async Task NextAsync()
	{
		this.DisableAllControls();
		this.StateHasChanged();

		var index = this.itemsRendered;

		if (index < this.items!.Count)
		{
			var item = this.items![index];
			await this.Binacle!.AddItemToSceneAsync(bin, item, index);
			this.itemsRendered += 1;
		}

		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task LastAsync()
	{
		this.DisableAllControls();
		this.StateHasChanged();
		await this.Binacle!.RedrawSceneAsync(this.bin!, this.items!);
		this.itemsRendered = this.items!.Count;
		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	
}
