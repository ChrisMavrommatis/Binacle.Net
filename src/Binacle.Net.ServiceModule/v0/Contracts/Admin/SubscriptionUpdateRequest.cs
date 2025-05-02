using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class SubscriptionUpdateRequest
{
	[Required]
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<SubscriptionType>>))]
	public SubscriptionType? Type { get; set; }

	[Required]
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<SubscriptionStatus>>))]
	public SubscriptionStatus? Status { get; set; }
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

internal class SubscriptionUpdateValidationProblemExample : ValidationProblemResponseExample
{
	// yield return OpenApiExample.Create(
	// "idparametererror",
	// "ID Parameter Error",
	// "Example response when you provide and ID that isn't Guid",
	// ErrorResponse.IdToGuidParameterError
	// );

	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>()
		{
			{ "Type", [ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(SubscriptionUpdateRequest.Type))] },
			{
				"Status",
				[ErrorMessage.RequiredEnumValues<SubscriptionStatus>(nameof(SubscriptionUpdateRequest.Status))]
			}
		};
	}
}
