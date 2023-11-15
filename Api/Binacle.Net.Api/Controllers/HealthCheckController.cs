using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.Controllers
{
    [ApiVersion("1.0")]
    public class HealthCheckController : VersionedApiControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Index()
        {
            return this.SectionAndVersionResult();
        }
    }
}
