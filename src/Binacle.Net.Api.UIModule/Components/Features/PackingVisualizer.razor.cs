using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class PackingVisualizer : ComponentBase
{
	[Parameter]
	public ViewModels.Bin? Bin { get; set; }
	
	[Parameter]
	public List<ViewModels.Item>? Items { get; set; }

	private Dictionary<string, Models.Control> controls = [];
	
	protected override void OnInitialized()
	{
		this.controls.Add("first", new Models.Control("control-first", "first_page", FirstAsync));
		this.controls.Add("previous", new Models.Control("control-previous", "chevron_left", PreviousAsync));
		this.controls.Add("repeat", new Models.Control("control-repeat", "repeat_one", RepeatAsync));
		this.controls.Add("next", new Models.Control("control-next", "chevron_right", NextAsync));
		this.controls.Add("last", new Models.Control("control-last", "last_page", LastAsync));
		
		base.OnInitialized();	
	}

	protected override async Task  OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await JS.InvokeVoidAsync("binacle.initialize", this.Bin);
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

