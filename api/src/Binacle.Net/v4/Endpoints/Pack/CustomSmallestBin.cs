using System.Net.Mime;
﻿using Binacle.Lib.Abstractions.Models;
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
			.WithOperationId("pack.customSmallestBin")
			.WithTags("Pack")
			.WithSummary("Pack the smallest custom bin")
			.WithDescription("Pack every custom bin and return the result for the smallest bin that can fit the items.")
			
			.Accepts<PackCustomSmallestBinRequest>(MediaTypeNames.Application.Json)
			.RequestExample<PackCustomSmallestBinRequestExample>(MediaTypeNames.Application.Json)
			
			.Produces<PackBinResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseDescription(StatusCodes.Status200OK, 
				"Returns the result of the packing operation for the smallest custom bin that can fit the items.")
			.ResponseExamples<PackCustomSmallestBinResponseExamples>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<CustomBinsValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
				MediaTypeNames.Application.ProblemJson
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal static async Task<IResult> HandleAsync(
		BindingResult<PackCustomSmallestBinRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<CustomSmallestBin> logger,
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
					request.Bins,
					request.Items,
					request.Parameters.ForPackingOperation(),
					cancellationToken
				);
			}
			else
			{
				result = await binacleService.SmallestBinAsync(
					request.Bins,
					request.Items,
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

