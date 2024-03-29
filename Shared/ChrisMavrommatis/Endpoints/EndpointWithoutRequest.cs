using Microsoft.AspNetCore.Mvc;

namespace ChrisMavrommatis.Endpoints;

public abstract class EndpointWithoutRequest<TResponse> : EndpointBase
{
	public abstract Task<ActionResult<TResponse>> HandleAsync(
		CancellationToken cancellationToken = default
		);
}

public abstract class EndpointWithoutRequest : EndpointBase
{
	public abstract Task<IActionResult> HandleAsync(
		CancellationToken cancellationToken = default
		);
}
