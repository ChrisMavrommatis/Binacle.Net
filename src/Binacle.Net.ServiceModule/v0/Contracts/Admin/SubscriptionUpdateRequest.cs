using System.Text.Json.Serialization;
using Binacle.Net.Kernel.OpenApi.Helpers;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class SubscriptionUpdateRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public required SubscriptionType? Type { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public required SubscriptionStatus? Status { get; set; }
}

internal class SubscriptionUpdateRequestValidator : AbstractValidator<SubscriptionUpdateRequest>
{
	public SubscriptionUpdateRequestValidator()
	{
		RuleFor(x => x.Type)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(SubscriptionUpdateRequest.Type)));

		RuleFor(x => x.Status)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<SubscriptionStatus>(nameof(SubscriptionUpdateRequest.Status)));
	}
}

internal class SubscriptionUpdateRequestExample : ISingleOpenApiExamplesProvider<SubscriptionUpdateRequest>
{
	public IOpenApiExample<SubscriptionUpdateRequest> GetExample()
	{
		return OpenApiExample.Create(
			"subscriptionUpdate",
			"Subscription Update",
			new SubscriptionUpdateRequest
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			}
		);
	}
}

internal class SubscriptionUpdateValidationProblemExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiValidationProblemExample.Create(
			"validationProblem",
			"Validation Problem",
			"Example response when you provide invalid Type and Status",
			new Dictionary<string, string[]>()
			{
				{ "Type", [ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(SubscriptionUpdateRequest.Type))] },
				{
					"Status",
					[ErrorMessage.RequiredEnumValues<SubscriptionStatus>(nameof(SubscriptionUpdateRequest.Status))]
				}
			}
		);

		yield return OpenApiValidationProblemExample.Create(
			"invalidId",
			"Invalid Id",
			"Example response when you provide and ID that isn't Guid",
			new Dictionary<string, string[]>()
			{
				{ "Id", [ErrorMessage.IdMustBeGuid] },
			}
		);
	}
}
