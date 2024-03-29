using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.Models.Requests;

public class PresetQueryRequestWithBody : RequestWithBody<PresetQueryRequest>
{
	public string Preset { get; set; }
}

public class PresetQueryRequest
{
	public List<Box> Items { get; set; }
}
