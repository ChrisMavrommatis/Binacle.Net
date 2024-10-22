using Binacle.Net.Api.UIModule.Models;

namespace Binacle.Net.Api.UIModule.ApiModels.Requests;

internal class PackByCustomRequest
{
	public required List<Bin> Bins { get; set; }
	public required List<Item> Items { get; set; }
}
