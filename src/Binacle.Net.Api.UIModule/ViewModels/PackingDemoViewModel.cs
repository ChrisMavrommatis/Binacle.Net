namespace Binacle.Net.Api.UIModule.ViewModels;

internal class PackingDemoViewModel
{
	public required ViewModels.Algorithm Algorithm { get; set; }
	public required List<ViewModels.Bin> Bins { get; set; }
	public required List<ViewModels.Item> Items { get; set; }
}
