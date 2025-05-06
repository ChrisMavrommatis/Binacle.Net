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

internal class SubscriptionCreateRequest
{
	[Required]
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public SubscriptionType? Type { get; set; }
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

internal class SubscriptionCreateValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		// yield return OpenApiExample.Create(
		// 	"idparametererror",
		// 	"ID Parameter Error",
		// 	ErrorResponse.IdToGuidParameterError
		// );

		return new Dictionary<string, string[]>()
		{
			{ "Type", [ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(Type))] }
		};
	}
}
