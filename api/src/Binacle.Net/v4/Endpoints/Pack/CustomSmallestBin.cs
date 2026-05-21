using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Pack;

internal class CustomSmallestBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/smallest-bin", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack the smallest bin")
			.WithDescription("Pack all bins and return the result for the smallest bin that can fit the items.")
			
			.Accepts<PackCustomSmallestBinRequest>("application/json")
			.RequestExample<PackCustomSmallestBinRequestExample>("application/json")
			
			.Produces<PackBinResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, 
				"Returns the result of the packing operation for the smallest custom bin and items.")
			.ResponseExamples<PackCustomSmallestBinResponseExamples>(StatusCodes.Status200OK, "application/json")
			
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
		BindingResult<PackCustomSmallestBinRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<CustomBin> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Custom Smallest Bin: v4");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			var algorithm = request.Parameters.GetAlgorithm();
			
			OperationResult result = null!;
			if (algorithm.HasValue)
			{
				result = await binacleService.SmallestBinAsync(
					algorithm.Value,
					request.Bins!,
					request.Items!,
					request.Parameters.ForPackingOperation()
				);
			}
			else
			{
				result = await binacleService.SmallestBinAsync(
					request.Bins!,
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

