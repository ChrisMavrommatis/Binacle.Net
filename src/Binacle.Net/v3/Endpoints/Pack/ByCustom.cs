using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Models;
using Binacle.Net.Services;
using Binacle.Net.v3.Contracts;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v3.Endpoints.Pack;

internal class ByCustom : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-custom", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Custom")
			.WithDescription("Pack items using custom bins")
			
			.Accepts<PackByCustomRequest>("application/json")
			.RequestExample<PackByCustomRequestExample>("application/json")
			
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200Ok)
			.ResponseExamples<PackByCustomResponseExamples>(StatusCodes.Status200OK, "application/json")
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<PackByCustomValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage");
	}

	internal async Task<IResult> HandleAsync(
		BindingResult<PackByCustomRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<ByCustom> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack by Custom: v3");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			var operationResults = await binacleService.PackBinsAsync(
				request.Bins!,
				request.Items!,
				new PackingParameters
				{
					Algorithm = request.Parameters!.Algorithm!.Value,
					OptInToEarlyFails = request.Parameters.OptInToEarlyFails,
					ReportPackedItemsOnlyWhenFullyPacked = request.Parameters.ReportPackedItemsOnlyWhenFullyPacked,
					NeverReportUnpackedItems = request.Parameters.NeverReportUnpackedItems
				}
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackResponse.Create(
						request.Bins!,
						request.Items!,
						request.Parameters,
						operationResults
					)
				);
			}
		});
	}
}

