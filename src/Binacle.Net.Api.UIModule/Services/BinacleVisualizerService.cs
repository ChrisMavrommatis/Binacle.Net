using Binacle.Net.Lib.Abstractions.Models;
using Microsoft.JSInterop;

namespace Binacle.Net.Api.UIModule.Services;

internal class BinacleVisualizerService
{
	private readonly IJSRuntime js;

	public BinacleVisualizerService(IJSRuntime js)
	{
		this.js = js;
	}
	public ValueTask InitializeSceneAsync(UIModule.Models.Bin bin)
	{
		return this.js.InvokeVoidAsync("binacle.initialize", bin);
	}

	public ValueTask UpdateLoadingStartAsync()
	{
		return this.js.InvokeVoidAsync("binacle.loadingStart");
	}

	public ValueTask UpdateLoadingEndAsync()
	{
		return this.js.InvokeVoidAsync("binacle.loadingEnd");
	}

	public ValueTask RedrawSceneAsync<TBin>(TBin bin, List<UIModule.Models.PackedItem>? items)
		where TBin : IWithID, IWithReadOnlyDimensions
	{
		return this.js.InvokeVoidAsync("binacle.redrawScene", bin, items);
	}

	public ValueTask AddItemToSceneAsync<TBin>(TBin bin, UIModule.Models.PackedItem item, int index)
	{
		return this.js.InvokeVoidAsync("binacle.addItemToScene", bin, item, index);
	}

	public ValueTask RemoveItemFromSceneAsync(int index)
	{
		return this.js.InvokeVoidAsync("binacle.removeItemFromScene", index);
	}
		
}
