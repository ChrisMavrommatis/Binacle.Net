using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.Models;
using Binacle.Net.v3.Models;

namespace Binacle.Net.v3.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomPackRequest
{
	public PackRequestParameters Parameters { get; set; } = new();
	public List<Bin> Bins { get; set; } = new();
	public List<Box> Items { get; set; } = new();
}

public class PackRequestParameters : IWithAlgorithm
{
	// TODO: Investigate this as with this it fails
	[Required]
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<Algorithm>>))]
	public Algorithm? Algorithm { get; set; }
}
