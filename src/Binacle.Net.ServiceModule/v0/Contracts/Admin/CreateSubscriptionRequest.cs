using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateSubscriptionRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<SubscriptionType>>))]
	public SubscriptionType? Type { get; set; }

	internal class Validator : AbstractValidator<CreateSubscriptionRequest>
	{
		public Validator()
		{
			RuleFor(x => x.Type)
				.NotNull()
				.WithMessage(ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(Type)));
		}
	}

	internal class Example : ISingleOpenApiExamplesProvider<CreateSubscriptionRequest>
	{
		public IOpenApiExample<CreateSubscriptionRequest> GetExample()
		{
			return OpenApiExample.Create(
				"createSubscription",
				"Create Subscription",
				new CreateSubscriptionRequest
				{
					Type = SubscriptionType.Normal
				}
			);
		}
	}

	internal class ErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
	{
		public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"idparametererror",
				"ID Parameter Error",
				ErrorResponse.IdToGuidParameterError
			);

			yield return OpenApiExample.Create(
				"validationError",
				"Validation Error",
				"Example response with validation errors",
				ErrorResponse.ValidationError(
				[
					ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(Type))
				])
			);
		}
	}
}
