using Binacle.Net.Api.v2.Models;
using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PresetQueryRequestWithBody : RequestWithBody<PresetQueryRequest>
{
	public string Preset { get; set; }
}

public class PresetQueryRequest
{
	public QueryRequestParameters? Parameters { get; set; }
	public List<Box> Items { get; set; }
}
