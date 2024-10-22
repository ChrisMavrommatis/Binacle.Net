using Binacle.Net.Api.v2.Models;
using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PresetFitRequestWithBody : RequestWithBody<PresetFitRequest>
{
	public string? Preset { get; set; }
}

public class PresetFitRequest
{
	public FitRequestParameters? Parameters { get; set; }
	public List<Box>? Items { get; set; }
}
