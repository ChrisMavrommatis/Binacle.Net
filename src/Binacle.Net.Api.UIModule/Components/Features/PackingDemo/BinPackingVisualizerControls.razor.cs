using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPackingVisualizerControls : ComponentBase
{
	[Inject]
	internal Services.PackingDemoState State { get; set; }

	private Dictionary<string, ViewModels.Control> controls = [];
	private int itemsRendered;
	private CancellationTokenSource cancellationTokenSource;
	private bool repeating;

	protected override void OnInitialized()
	{
		// register on state.results change to update the state

		this.controls.Add("first", new ViewModels.Control("control-first", "first_page", FirstAsync));
		this.controls.Add("previous", new ViewModels.Control("control-previous", "chevron_left", PreviousAsync));
		this.controls.Add("repeat", new ViewModels.Control("control-repeat", "repeat_one", RepeatAsync));
		this.controls.Add("next", new ViewModels.Control("control-next", "chevron_right", NextAsync));
		this.controls.Add("last", new ViewModels.Control("control-last", "last_page", LastAsync));

		base.OnInitialized();

		this.itemsRendered = 0;
		State.OnResultsChanged += ResultsChanged;
		State.OnSelectResult += SelectionChanged;
	}


	private Task SelectionChanged(Models.PackingResult result)
	{
		this.DisableAllControls();

		this.itemsRendered = result.PackedItems!.Count;

		this.UpdateControlsStatus();

		this.StateHasChanged();
		return Task.CompletedTask;
	}

	private Task ResultsChanged(ReadOnlyCollection<Models.PackingResult> collection)
	{
		this.itemsRendered = 0;
		this.DisableAllControls();
		this.StateHasChanged();
		return Task.CompletedTask;
	}

	private void DisableAllControls()
	{
		foreach (var control in this.controls.Values)
		{
			control.IsEnabled = false;
		}
	}


	private void UpdateControlsStatus()
	{
		if (this.State.SelectedResult is null)
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

		if (this.itemsRendered >= this.State.SelectedResult!.PackedItems!.Count)
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


	private async Task FirstAsync()
	{
		this.DisableAllControls();
		this.StateHasChanged();
		await this.State.RedrawSceneAsync(this.State.SelectedResult!.Bin, null);
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
			await this.State.RemoveItemFromScene(index);
			this.itemsRendered -= 1;
		}
	
		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task RepeatAsync()
	{

		if (this.repeating)
		{
			await this.cancellationTokenSource.CancelAsync();
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

		await this.State.RedrawSceneAsync(this.State.SelectedResult!.Bin, null);

		await Task.Run(async () =>
		{
			if (!this.cancellationTokenSource.Token.IsCancellationRequested)
			{
				var bin = this.State.SelectedResult!.Bin;
				for (var i = 0; i < this.State.SelectedResult.PackedItems.Count; i++)
				{
					var item = this.State.SelectedResult!.PackedItems![i];
					await this.State.AddItemToScene(bin, item, i);
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

		if (index < this.State.SelectedResult!.PackedItems!.Count)
		{
			var bin = this.State.SelectedResult!.Bin;
			var item = this.State.SelectedResult!.PackedItems![index];
			await this.State.AddItemToScene(bin, item, index);
			this.itemsRendered += 1;
		}

		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

	private async Task LastAsync()
	{
		this.DisableAllControls();
		this.StateHasChanged();
		await this.State.RedrawSceneAsync(this.State.SelectedResult!.Bin, this.State.SelectedResult.PackedItems);
		this.itemsRendered = this.State.SelectedResult!.PackedItems!.Count;
		this.UpdateControlsStatus();
		this.StateHasChanged();
	}

}

