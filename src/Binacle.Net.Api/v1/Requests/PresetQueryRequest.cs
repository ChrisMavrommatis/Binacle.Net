using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v1.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PresetQueryRequestWithBody : RequestWithBody<PresetQueryRequest>
{
	public string? Preset { get; set; }
}

public class PresetQueryRequest
{
	public List<v1.Models.Box>? Items { get; set; }
}
