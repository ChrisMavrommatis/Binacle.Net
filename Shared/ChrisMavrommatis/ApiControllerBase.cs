using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class ApiControllerBase: ControllerBase
{
	/// <summary>
	/// Creates an <see cref="InternalServerErrorObjectResult"/> that produces a <see cref="StatusCodes.Status500InternalServerError"/> response.
	/// </summary>
	/// <param name="error">An error object to be returned to the client.</param>
	/// <returns>The created <see cref="InternalServerErrorObjectResult"/> for the response.</returns>
	[NonAction]
	public virtual InternalServerErrorObjectResult InternalServerError([ActionResultObjectValue] object? error)
		=> new InternalServerErrorObjectResult(error);
}
