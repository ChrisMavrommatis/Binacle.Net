using Microsoft.AspNetCore.Mvc;

namespace ChrisMavrommatis.Endpoints;

public abstract class EndpointWithRequest<TRequest, TResponse> : EndpointBase
{
	public abstract Task<ActionResult<TResponse>> HandleAsync(
		TRequest request,
		CancellationToken cancellationToken = default
		);
}

public abstract class EndpointWithRequest<TRequest> : EndpointBase
{
	public abstract Task<IActionResult> HandleAsync(
		TRequest request,
		CancellationToken cancellationToken = default
		);
}
