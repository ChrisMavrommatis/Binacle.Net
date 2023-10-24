using Binacle.Api.Components.Api.Responses;
using Binacle.Api.Components.Controllers;
using Binacle.Api.Components.Services;
using Binacle.Api.Glockers.Models;
using Binacle.Api.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Api.Controllers
{
    [ApiVersion("1.0")]
    public class CustomController : ApiControllerBase
    {
        private readonly IValidator<QueryRequest> validator;
        private readonly ILockerService lockerService;
        public CustomController(
          IValidator<QueryRequest> validator,
          ILockerService lockerService
          )
        {
            this.validator = validator;
            this.lockerService = lockerService;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Query(QueryRequest request)
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

            var items = request.GetItemsForService();
            var bins = request.GetBinsForService();
            var result = await this.lockerService.FindFittingBinAsync(bins, items);

            return this.Ok(QueryResponse.CreateFrom(result));
        }
    }
}
