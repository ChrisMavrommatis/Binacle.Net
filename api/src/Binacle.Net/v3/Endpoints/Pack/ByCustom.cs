using System.Net.Mime;
﻿using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v3.Contracts;
using Binacle.Net.v3.ExtensionMethods;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v3.Endpoints.Pack;

internal class ByCustom : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-custom", HandleAsync)
			.WithOperationId("packByCustom")
			.WithTags("Pack")
			.WithSummary("Pack by custom")
			.WithDescription("Pack items using custom bins.")
			
			.Accepts<PackByCustomRequest>(MediaTypeNames.Application.Json)
			.RequestExample<PackByCustomRequestExample>(MediaTypeNames.Application.Json)
			
			.Produces<PackResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200Ok)
			.ResponseExamples<PackByCustomResponseExamples>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<PackByCustomValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				MediaTypeNames.Application.ProblemJson
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal static async Task<IResult> HandleAsync(
		BindingResult<PackByCustomRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<ByCustom> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack by Custom: v3");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			var operationResults = await binacleService.MultipleBinsAsync(
				request.Parameters.Algorithm.ToLibAlgorithm(),
				request.Bins,
				request.Items,
				request.Parameters,
				cancellationToken
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackResponse.Create(
						request.Bins,
						request.Items,
						request.Parameters,
						operationResults
					)
				);
			}
		});
	}
}

