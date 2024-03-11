using Binacle.Net.Api.Examples;
using Binacle.Net.Api.Models;
using ChrisMavrommatis.Api.Endpoints.Requests;

namespace Binacle.Net.Api.Endpoints.Query;

public class PresetQueryRequestWithBody : RequestWithBody<PresetQueryRequest>
{
	public string Preset { get; set; }
}

public class PresetQueryRequest : IWithSwaggerExample<PresetQueryRequestExampleHolder>
{
	public List<Box> Items { get; set; }
}
