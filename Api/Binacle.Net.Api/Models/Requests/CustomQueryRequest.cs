using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.Models.Requests;

public class CustomQueryRequestWithBody : RequestWithBody<CustomQueryRequest>
{
}
public class CustomQueryRequest
{
	public List<Bin> Bins { get; set; }
	public List<Box> Items { get; set; }
}
