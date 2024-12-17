using System.Text.Json.Serialization;
using Binacle.Net.Api.Kernel.Serialization;
using Binacle.Net.Api.Models;
using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v3.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomPackRequestWithBody : RequestWithBody<CustomPackRequest>
{
}

public class CustomPackRequest
{
	public PackRequestParameters? Parameters { get; set; }
	public List<v3.Models.Bin>? Bins { get; set; }
	public List<v3.Models.Box>? Items { get; set; }
}

public class PackRequestParameters
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<Algorithm>>))]
	public Algorithm? Algorithm { get; set; }
	public bool? OptInToEarlyFails { get; set; }
	public bool? ReportPackedItemsOnlyWhenFullyPacked { get; set; }
	public bool? NeverReportUnpackedItems { get; set; }
}
