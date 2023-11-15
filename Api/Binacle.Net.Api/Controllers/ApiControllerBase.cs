using Binacle.Net.Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        [NonAction]
        public ApiErrorResponse ValidationErrorResponse(string message = "One or More Validation errors occurred.")
        {
            var errors = new List<IApiError>();
            foreach (var kvp in this.ModelState)
            {
                if (kvp.Value?.ValidationState == ModelValidationState.Invalid)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        errors.Add(new FieldError() { Field = kvp.Key, Mesasage = error.ErrorMessage });
                    }
                }
            }

            return new ApiErrorResponse
            {
                Message = message,
                Errors = errors
            };
        }

        [NonAction]
        public IActionResult SectionAndVersionResult([CallerMemberName] string caller = "")
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

        protected void AddModelStateError(string propertyName, string message)
        {
            this.ModelState.AddModelError(propertyName, message);
        }
    }
}
