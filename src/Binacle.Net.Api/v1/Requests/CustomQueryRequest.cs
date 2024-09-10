using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v1.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomQueryRequestWithBody : RequestWithBody<CustomQueryRequest>
{
}
public class CustomQueryRequest
{
	public List<v1.Models.Bin> Bins { get; set; }
	public List<v1.Models.Box> Items { get; set; }
}
