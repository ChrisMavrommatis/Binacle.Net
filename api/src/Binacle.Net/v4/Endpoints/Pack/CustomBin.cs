using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Pack;

internal class CustomBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/bin", HandleAsync)
			.WithOperationId("pack.customBin")
			.WithTags("Pack")
			.WithSummary("Pack a custom bin")
			.WithDescription("Pack items into a custom bin.")
			
			.Accepts<PackCustomBinRequest>("application/json")
			.RequestExample<PackCustomBinRequestExample>("application/json")
			
			.Produces<PackBinResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, 
				"Returns the result of the packing operation for the specified custom bin and items.")
			.ResponseExamples<PackCustomBinResponseExamples>(StatusCodes.Status200OK, "application/json")
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<CustomBinValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
			 "application/problem+json"
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal async Task<IResult> HandleAsync(
		BindingResult<PackCustomBinRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<CustomBin> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Custom Bin: v4");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			var algorithm = request.Parameters.GetAlgorithm();
			
			OperationResult result = null!;
			if (algorithm.HasValue)
			{
				result = await binacleService.SingleBinAsync(
					algorithm.Value,
					request.Bin!,
					request.Items!,
					request.Parameters.ForPackingOperation(),
					cancellationToken
				);
			}
			else
			{
				result = await binacleService.SingleBinAsync(
					request.Bin!,
					request.Items!,
					request.Parameters.ForPackingOperation(),
					cancellationToken
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

