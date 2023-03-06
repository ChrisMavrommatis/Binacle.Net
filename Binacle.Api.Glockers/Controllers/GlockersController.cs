using Binacle.Api.Components.Api.Responses;
using Binacle.Api.Components.Controllers;
using Binacle.Api.Components.Services;
using Binacle.Api.Glockers.Extensions;
using Binacle.Api.Glockers.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Api.Glockers.Controllers
{
    [ApiVersion("1.0")]
    public class GlockersController : ApiControllerBase
    {
        private readonly IOptions<GlockersOptions> options;
        private readonly IValidator<GlockersQueryRequest> validator;
        private readonly ILockerService lockerService;

        public GlockersController(
            IOptions<GlockersOptions> options,
            IValidator<GlockersQueryRequest> validator,
            ILockerService lockerService
            )
        {
            this.options = options;
            this.validator = validator;
            this.lockerService = lockerService;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult ConfiguredLockerSizes()
        {
            return this.Ok(this.options.Value.Lockers);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GlockersQueryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Query(GlockersQueryRequest request)
        {
            var validationResult = await this.validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    this.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return this.ValidationError(validationResult);
            }

            var result = await this.lockerService.FindFittingBinAsync(
                this.options.Value.GetBinsForService(),
                request.GetItemsForService()
                );

            return this.Ok(GlockersQueryResponse.CreateFrom(result));
        }
    }
}

