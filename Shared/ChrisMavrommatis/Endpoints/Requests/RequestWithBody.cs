using Microsoft.AspNetCore.Mvc;

namespace ChrisMavrommatis.Endpoints.Requests;

public abstract class RequestWithBody<TBody>
{
	[FromBody]
	public TBody Body { get; set; }
}
