using Asp.Versioning;
using Binacle.Net.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
	[NonAction]
	protected ObjectResult InternalServerError(object objectDetails)
	{
		return new ObjectResult(objectDetails)
		{
			StatusCode = StatusCodes.Status500InternalServerError
		};
	}

	[NonAction]
	protected ErrorResponse ErrorResponse(string message)
	{
		return new ErrorResponse
		{
			Message = message
		};
	}

	[NonAction]
	protected IActionResult SectionAndVersionResult([CallerMemberName] string caller = "")
	{
		string? version = null;
		var controllerType = this.GetType();
		var section = controllerType.Name.Replace("Controller", string.Empty);
		if (!string.IsNullOrEmpty(caller))
		{
			var method = controllerType.GetMethod(caller);
			if (method != null)
			{
				var attribute = method.GetCustomAttribute<MapToApiVersionAttribute>(inherit: true);
				if (attribute != null)
				{
					version = attribute.Versions[0]?.ToString();
				}
			}
		}

		return this.Ok(new HealthCheckResponse(version ?? "N/A", section));
	}

	protected void AddModelStateError(string propertyName, string message)
	{
		this.ModelState.AddModelError(propertyName, message);
	}
}
