
using Binacle.Net.v2.Models;

namespace Binacle.Net.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomPackRequest : IWithPackRequestParameters
{
	public PackRequestParameters? Parameters { get; set; }
	public List<Bin>? Bins { get; set; }
	public List<Box>? Items { get; set; }
}
