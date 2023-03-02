using Binacle.Api.BoxNow.Requests;
using Binacle.Api.BoxNow.Responses;
using Binacle.Api.BoxNow.Configuration;
using Binacle.Api.Components.Api.Responses;
using Binacle.Api.Components.Controllers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Api.BoxNow.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class BoxNowController : ApiControllerBase
    {
        private readonly IOptions<BoxNowOptions> options;
        private readonly IValidator<BoxNowLockerQueryRequest> validator;

        public BoxNowController(
            IOptions<BoxNowOptions> options, 
            IValidator<BoxNowLockerQueryRequest> validator
            )
        {
            this.options = options;
            this.validator = validator;
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
            return this.Ok(this.options.Value.Lockers);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BoxNowLockerQueryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Query(BoxNowLockerQueryRequest request)
        {
            var result = await this.validator.ValidateAsync(request);
            if (!result.IsValid)
            {
               foreach(var error in result.Errors)
                {
                    this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return this.ValidationError(result);
            }
            return this.Ok(new BoxNowLockerQueryResponse()
            {
                Locker = this.options.Value.Lockers.FirstOrDefault()
            });
        }

        [NonAction]
        public IActionResult ValidationError(ValidationResult result)
        {
            return this.BadRequest(new ApiErrorResponse
            {
                Errors = result.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToList()
            });
        }
    }
}
