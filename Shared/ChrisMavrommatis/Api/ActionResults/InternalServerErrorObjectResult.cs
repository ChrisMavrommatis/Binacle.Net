using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ChrisMavrommatis.Api.ActionResults;

/// <summary>
/// An <see cref="ObjectResult"/> that when executed will produce a Internal Server Error (500) response.
/// </summary>
[DefaultStatusCode(DefaultStatusCode)]
public class InternalServerErrorObjectResult : ObjectResult
{
	private const int DefaultStatusCode = StatusCodes.Status500InternalServerError;

	/// <summary>
	/// Creates a new <see cref="InternalServerErrorObjectResult"/> instance.
	/// </summary>
	/// <param name="error">Contains the errors to be returned to the client.</param>
	public InternalServerErrorObjectResult([ActionResultObjectValue] object? error)
		: base(error)
	{
		StatusCode = DefaultStatusCode;
	}
}
