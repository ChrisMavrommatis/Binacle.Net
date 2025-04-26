using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

#pragma warning disable CS8618
internal class PartialUpdateSubscriptionRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<SubscriptionType>>))]
	public SubscriptionType? Type { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<SubscriptionStatus>>))]
	public SubscriptionStatus? Status { get; set; }

	internal class Validator : AbstractValidator<PartialUpdateSubscriptionRequest>
	{
		public Validator()
		{
			RuleFor(x => x)
				.Must(x =>
					x.Status.HasValue
					|| x.Type.HasValue
				).WithMessage(
					"At least one field must be provided for update."
				);
		}
	}


	internal class Examples : IMultipleOpenApiExamplesProvider<PartialUpdateSubscriptionRequest>
	{
		public IEnumerable<IOpenApiExample<PartialUpdateSubscriptionRequest>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"fullUpdate",
				"Full Update",
				new PartialUpdateSubscriptionRequest
				{
					Type = SubscriptionType.Normal,
					Status = SubscriptionStatus.Suspended
				}
			);
			
			yield return OpenApiExample.Create(
				"onlyType",
				"Only Type",
				new PartialUpdateSubscriptionRequest
				{
					Type = SubscriptionType.Normal,
				}
			);
			
			yield return OpenApiExample.Create(
				"onlyStatus",
				"Only Status",
				new PartialUpdateSubscriptionRequest
				{
					Status = SubscriptionStatus.Suspended
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
					"At least one field must be provided for update."
				])
			);
		}
	}
}
