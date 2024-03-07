using Microsoft.AspNetCore.Mvc;

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
}
