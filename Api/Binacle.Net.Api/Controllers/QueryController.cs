using Asp.Versioning;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Options.Models;
using Binacle.Net.Api.Responses;
using Binacle.Net.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Binacle.Net.Api.Controllers
{
    [ApiVersion("1.0")]
    public class QueryController : VersionedApiControllerBase
    {
        private readonly IValidator<QueryRequest> queryRequestValidator;
        private readonly IValidator<PresetQueryRequest> presetQueryRequestValidator;
        private readonly IOptions<BinPresetOptions> presetOptions;
        private readonly ILockerService lockerService;
        public QueryController(
            IValidator<QueryRequest> queryRequestValidator,
            IValidator<PresetQueryRequest> presetQueryRequestValidator,
            IOptions<BinPresetOptions> presetOptions,
            ILockerService lockerService
          )
        {
            this.queryRequestValidator = queryRequestValidator;
            this.presetQueryRequestValidator = presetQueryRequestValidator;
            this.presetOptions = presetOptions;
            this.lockerService = lockerService;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Index(QueryRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ValidationErrorResponse());
            }

            var validationResult = await this.queryRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    this.AddModelStateError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ValidationErrorResponse());
            }

            var response = this.lockerService.FindFittingBin(request.Containers, request.Items);


            return this.Ok(response);
        }

        /// <summary>
        /// Perform a bin fit query using a specified bin preset.
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="request"></param>
        /// <returns>The bin that fits all of the items</returns>
        /// <remarks>
        /// Example request using the "sample" preset:
        ///     
        ///     POST /api/v1/query/presets/sample
        ///     {
        ///         "items": [
        ///           {
        ///             "id": "box_1",
        ///             "quantity": 2,
        ///             "length": 2,
        ///             "width": 5,
        ///             "height": 10
        ///           },
        ///           {
        ///             "id": "box_2",
        ///             "quantity": 1,
        ///             "length": 12,
        ///             "width": 15,
        ///             "height": 10
        ///           }
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Returns the bin that fits all of the items</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the preset does not exist</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpPost]
        [Route("presets/{preset}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // Swashbuckle.AspNetCore.Filters
        // [SwaggerRequestExample(typeof(PresetQueryRequest), typeof(PresetQueryRequestExampleProvider))]
        public async Task<IActionResult> Preset(
            [DefaultValue("sample")] string preset, 
            [FromBody] PresetQueryRequest request
            )
        {
            if (string.IsNullOrWhiteSpace(preset))
            {
                this.AddModelStateError(nameof(preset), Constants.ErrorMessages.IsRequired);
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ValidationErrorResponse());
            }

            var validationResult = await this.presetQueryRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    this.AddModelStateError(error.PropertyName, error.ErrorMessage);
                }
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ValidationErrorResponse());
            }

            if (!this.presetOptions.Value.Presets.TryGetValue(preset, out var presetOption))
            {
                this.AddModelStateError(nameof(preset), string.Format("preset '{0}' does not exist.", preset));
                return this.NotFound(this.ValidationErrorResponse("preset not found."));
            }

            var response = this.lockerService.FindFittingBin(presetOption.Bins, request.Items);

            return this.Ok(response);
        }

        [HttpGet]
        [Route("presets")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> PresetsList()
        {
            return this.Ok(this.presetOptions.Value.Presets);
        }
    }
}
