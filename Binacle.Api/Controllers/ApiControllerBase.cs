using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Binacle.Api.Controllers
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

            return this.Ok(new Responses.HealthCheckResponse(version ?? "N/A", section));
        }
    }
}
