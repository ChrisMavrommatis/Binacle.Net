using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.Models;

namespace Binacle.Net.v3.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomPackRequest
{
	public PackRequestParameters? Parameters { get; set; }
	public List<v3.Models.Bin>? Bins { get; set; }
	public List<v3.Models.Box>? Items { get; set; }
}

public class PackRequestParameters : IWithAlgorithm
{
	// TODO: Investigate this as with this it fails
	// [JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<Algorithm>>))]
	public Algorithm? Algorithm { get; set; }
}
