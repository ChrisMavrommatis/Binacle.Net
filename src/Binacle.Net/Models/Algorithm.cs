using System.Text.Json.Serialization;

namespace Binacle.Net.Models;

#pragma warning disable CS1591
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Algorithm
{
	FFD,
	WFD,
	BFD
}
