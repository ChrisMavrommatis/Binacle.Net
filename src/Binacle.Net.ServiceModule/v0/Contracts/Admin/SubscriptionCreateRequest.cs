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

internal class SubscriptionCreateRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public required SubscriptionType? Type { get; set; }
}

internal class SubscriptionCreateRequestValidator : AbstractValidator<SubscriptionCreateRequest>
{
	public SubscriptionCreateRequestValidator()
	{
		RuleFor(x => x.Type)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(Type)));
	}
}

internal class SubscriptionCreateRequestExample : ISingleOpenApiExamplesProvider<SubscriptionCreateRequest>
{
	public IOpenApiExample<SubscriptionCreateRequest> GetExample()
	{
		return OpenApiExample.Create(
			"subscriptionCreate",
			"Subscription Create",
			new SubscriptionCreateRequest
			{
				Type = SubscriptionType.Normal
			}
		);
	}
}

internal class SubscriptionCreateValidationProblemExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiValidationProblemExample.Create(
			"validationProblem",
			"Validation Problem",
			"Example response when you provide invalid type",
			new Dictionary<string, string[]>()
			{
				{ "Type", [ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(Type))] },
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
