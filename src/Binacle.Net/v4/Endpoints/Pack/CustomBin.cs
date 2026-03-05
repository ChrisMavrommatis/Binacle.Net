using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts.Pack;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Pack;

internal class CustomBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/bin", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack a Custom Bin")
			.WithDescription("Pack items into a custom bin.")
			
			.Accepts<PackCustomBinRequest>("application/json")
			.RequestExample<PackCustomBinRequestExample>("application/json")
			
			.Produces<BinPackResponse>(StatusCodes.Status200OK, "application/json")
			// .ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200Ok)
			// .ResponseExamples<PackByCustomResponseExamples>(StatusCodes.Status200OK, "application/json")
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			// .ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			//.ResponseExamples<PackByCustomValidationProblemExamples>(
			//	StatusCodes.Status422UnprocessableEntity,
			// "application/problem+json"
			//)
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
					request.Parameters.UsedForPack()
				);
			}
			else
			{
				result = await binacleService.SingleBinAsync(
					request.Bin!,
					request.Items!,
					request.Parameters.UsedForPack()
				);
			}
			
			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					BinPackResponse.Create(
						request.Bin!,
						request.Items!,
						request.Parameters,
						result
					)
				);
			}
		});
	}
}

