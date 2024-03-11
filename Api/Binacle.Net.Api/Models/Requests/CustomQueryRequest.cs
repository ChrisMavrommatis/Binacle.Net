using Binacle.Net.Api.Models;
using ChrisMavrommatis.Api.Endpoints.Requests;

namespace Binacle.Net.Api.Endpoints.Query;

public class CustomQueryRequestWithBody : RequestWithBody<CustomQueryRequest>
{
}
public class CustomQueryRequest
{
	public List<Bin> Bins { get; set; }
	public List<Box> Items { get; set; }
}
