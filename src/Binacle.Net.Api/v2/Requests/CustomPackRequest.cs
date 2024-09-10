using Binacle.Net.Api.v2.Models;
using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v2.Requests;

public class CustomPackRequestWithBody : RequestWithBody<CustomPackRequest>
{
}

public class CustomPackRequest
{
	public List<Bin> Bins { get; set; }
	public List<Box> Items { get; set; }
}
