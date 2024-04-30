using Binacle.Net.Api.Models;
using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v1.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomQueryRequestWithBody : RequestWithBody<CustomQueryRequest>
{
}
public class CustomQueryRequest
{
	public List<Bin> Bins { get; set; }
	public List<Box> Items { get; set; }
}
