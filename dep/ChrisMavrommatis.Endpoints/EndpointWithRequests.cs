using Microsoft.AspNetCore.Mvc;

namespace ChrisMavrommatis.Endpoints;

public abstract class EndpointWithRequests<TRequest1, TRequest2, TResponse> : EndpointBase
{
	public abstract Task<ActionResult<TResponse>> HandleAsync(
		TRequest1 parameter,
		TRequest2 request,
		CancellationToken cancellationToken = default
		);
}

public abstract class EndpointWithRequests<TRequest1, TRequest2> : EndpointBase
{
	public abstract Task<IActionResult> HandleAsync(
		TRequest1 parameter,
		TRequest2 request,
		CancellationToken cancellationToken = default
		);
}
