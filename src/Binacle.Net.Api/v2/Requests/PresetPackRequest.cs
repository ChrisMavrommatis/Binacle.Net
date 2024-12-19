using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PresetPackRequestWithBody : RequestWithBody<PresetPackRequest>
{
	public string? Preset { get; set; }
}

public class PresetPackRequest : IWithPackRequestParameters
{
	public PackRequestParameters? Parameters { get; set; }
	public List<v2.Models.Box>? Items { get; set; }
}
