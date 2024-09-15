using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.ApiModels.Requests;

internal class PackedItem : IWithID
{
	public string ID { get; set; } = string.Empty;

	public Dimensions Dimensions { get; set; }
	public Coordinates? Coordinates { get; set; }
}
