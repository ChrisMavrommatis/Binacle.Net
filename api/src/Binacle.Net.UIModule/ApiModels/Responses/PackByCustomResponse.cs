using System.Text.Json.Serialization;
using Binacle.Net.UIModule.Models;

namespace Binacle.Net.UIModule.ApiModels.Responses;

internal class PackByCustomResponse
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required ResponseResultType Result { get; set; }
	public required List<PackingResult> Data { get; set; }
}
