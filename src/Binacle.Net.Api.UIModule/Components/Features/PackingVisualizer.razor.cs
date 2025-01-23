using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class PackingVisualizer : ComponentBase
{
	[Parameter]
	public ViewModels.Bin? Bin { get; set; }
	
	// [Parameter] 
	// public EventCallback<ViewModels.Bin?> BinChanged { get; set; }
	//
	[Parameter]
	public List<ViewModels.Item>? Items { get; set; }
	
	// [Parameter] 
	// public EventCallback<List<ViewModels.Item>?> ItemsChanged { get; set; }
}

