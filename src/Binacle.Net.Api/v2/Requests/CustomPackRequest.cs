using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomPackRequestWithBody : RequestWithBody<CustomPackRequest>
{
}

public class CustomPackRequest : IWithPackRequestParameters
{
	public PackRequestParameters? Parameters { get; set; }
	public List<v2.Models.Bin>? Bins { get; set; }
	public List<v2.Models.Box>? Items { get; set; }
}
