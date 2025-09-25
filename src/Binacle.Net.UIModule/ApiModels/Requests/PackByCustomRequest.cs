using System.Text.Json.Serialization;
using Binacle.Net.UIModule.Models;

namespace Binacle.Net.UIModule.ApiModels.Requests;

internal class PackByCustomRequest
{
	public required PackRequestParameters Parameters { get; set; }
	public required List<Bin> Bins { get; set; }
	public required List<Item> Items { get; set; }
}

internal class PackRequestParameters
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required Algorithm Algorithm { get; init; }
}

