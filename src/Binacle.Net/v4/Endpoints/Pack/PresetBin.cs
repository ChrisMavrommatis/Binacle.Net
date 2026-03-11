using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Pack;

internal class PresetBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/bin/{preset}/{bin}", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack a bin from a preset")
			.WithDescription("Pack items into a bin from a preset. The preset and bin must be specified in the URL path.")
			
			.Accepts<PackPresetBinRequest>("application/json")
			.RequestExample<PackPresetBinRequestExample>("application/json")
			
			.Produces<PackBinResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, 
				"Returns the result of the packing operation for the specified bin and items.")
			.ResponseExamples<PackPresetBinResponseExamples>(StatusCodes.Status200OK, "application/json")
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(
				StatusCodes.Status404NotFound,
				"If the preset or the bin in the specified preset does not exist."
			)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<PresetBinValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}
	
	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		[FromRoute] string bin,
		BindingResult<PackPresetBinRequest> bindingResult,
		IOptions<BinPresetOptions> presetOptions,
		IBinacleService binacleService,
		ILogger<PresetBin> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Preset Bin: v4");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			if (!presetOptions.Value.TryGetPresetBin(preset, bin, out var binOption))
			{
				return Results.NotFound(null);
			}
			
			var algorithm = request.Parameters.GetAlgorithm();
			
			OperationResult result = null!;
			if (algorithm.HasValue)
			{
				result = await binacleService.SingleBinAsync(
					algorithm.Value,
					binOption,
					request.Items!,
					request.Parameters.ForPackingOperation()
				);
			}
			else
			{
				result = await binacleService.SingleBinAsync(
					binOption,
					request.Items!,
					request.Parameters.ForPackingOperation()
				);
			}
			
			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackBinResponse.From(
						request.Parameters,
						result
					)
				);
			}
		});
	}
}
