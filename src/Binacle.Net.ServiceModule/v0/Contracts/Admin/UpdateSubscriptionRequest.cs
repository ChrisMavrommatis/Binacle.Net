using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class UpdateSubscriptionRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<SubscriptionType>>))]
	public SubscriptionType? Type { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<SubscriptionStatus>>))]
	public SubscriptionStatus? Status { get; set; }

	internal class Validator : AbstractValidator<UpdateSubscriptionRequest>
	{
		public Validator()
		{
			RuleFor(x => x.Type)
				.NotNull()
				.WithMessage(ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(Type)));

			RuleFor(x => x.Status)
				.NotNull()
				.WithMessage(ErrorMessage.RequiredEnumValues<SubscriptionStatus>(nameof(Status)));
		}
	}


	internal class Example : ISingleOpenApiExamplesProvider<UpdateSubscriptionRequest>
	{
		public IOpenApiExample<UpdateSubscriptionRequest> GetExample()
		{
			return OpenApiExample.Create(
				"updateSubscriptionRequest",
				"Update Subscription Request",
				new UpdateSubscriptionRequest
				{
					Type = SubscriptionType.Normal,
					Status = SubscriptionStatus.Active
				}
			);
		}
	}

	internal class ErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
	{
		public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"malformedRequest",
				"Malformed Request",
				"Example response when the request is has some syntax errors",
				ErrorResponse.MalformedRequest
			);
			yield return OpenApiExample.Create(
				"idparametererror",
				"ID Parameter Error",
				"Example response when you provide and ID that isn't Guid",
				ErrorResponse.IdToGuidParameterError
			);

			yield return OpenApiExample.Create(
				"validationError",
				"Validation Error",
				"Example response with validation errors",
				ErrorResponse.ValidationError(
				[
					ErrorMessage.RequiredEnumValues<SubscriptionType>(nameof(Type)),
					ErrorMessage.RequiredEnumValues<SubscriptionStatus>(nameof(Status))
				])
			);
		}
	}
}
