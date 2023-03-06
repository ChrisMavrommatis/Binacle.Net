using Binacle.Api.Components.Api.Responses;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Binacle.Api.Components.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        [NonAction]
        public IActionResult HealthCheckResult([CallerMemberName] string caller = "")
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

            return this.Ok(new Api.Responses.HealthCheckResponse(version ?? "N/A", section));
        }


        [NonAction]
        public IActionResult ValidationError(ValidationResult validationResult)
        {
            return this.BadRequest(new ApiErrorResponse
            {
                Message = "One or More Validation errors occured",
                Errors = validationResult.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToList()
            });
        }
    }
}
