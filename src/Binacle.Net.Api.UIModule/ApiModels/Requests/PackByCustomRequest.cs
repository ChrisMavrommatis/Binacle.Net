using System.Text.Json.Serialization;
using Binacle.Net.Api.Kernel.Serialization;
using Binacle.Net.Api.UIModule.Models;

namespace Binacle.Net.Api.UIModule.ApiModels.Requests;

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
	public bool? OptInToEarlyFails { get; set; }
	public bool? ReportPackedItemsOnlyWhenFullyPacked { get; set; }
	public bool? NeverReportUnpackedItems { get; set; }
	public bool? StopAtSmallestBin { get; set; }
}

