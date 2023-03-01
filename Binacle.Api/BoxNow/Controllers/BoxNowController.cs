using Binacle.Api.BoxNow.Models;
using Binacle.Api.Controllers;
using Binacle.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Api.BoxNow.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class BoxNowController : ApiControllerBase
    {
        private readonly IOptions<BoxNowConfiguration> _options;
        private readonly ILockerService _lockerService;


        public BoxNowController(IOptions<BoxNowConfiguration> options, ILockerService lockerService)
        {
            this._options = options;
            this._lockerService = lockerService;
        }

        [HttpGet]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HealthCheck()
        {
            return this.HealthCheckResult();
        }

        [HttpGet]
        public IActionResult LockerSizesConfiguration()
        {
            return this.Ok(this._options.Value.Lockers);
        }

    }
}
