using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPackingForm : ComponentBase
{
	[Inject]
	internal Services.PackingDemoState State { get; set; }

	protected override void OnInitialized()
	{
		State.InitializeModelWithSampleData();
	}


	protected void AddItem()
	{
		this.State.Model.Items.Add(new ViewModels.Item(0, 0, 0, 1));
	}

	protected void RemoveItem(ViewModels.Item item)
	{
		this.State.Model.Items.Remove(item);
	}

	protected void ClearAllItems()
	{
		this.State.Model.Items.Clear();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await State.InitializeDomAsync();
		}
	}

	

}
