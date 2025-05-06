using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class SubscriptionPatchRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public SubscriptionType? Type { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public SubscriptionStatus? Status { get; set; }
}

internal class SubscriptionPatchRequestValidator : AbstractValidator<SubscriptionPatchRequest>
{
	public SubscriptionPatchRequestValidator()
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

internal class SubscriptionPatchRequestExamples : IMultipleOpenApiExamplesProvider<SubscriptionPatchRequest>
{
	public IEnumerable<IOpenApiExample<SubscriptionPatchRequest>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullUpdate",
			"Full Update",
			new SubscriptionPatchRequest
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Suspended
			}
		);

		yield return OpenApiExample.Create(
			"onlyType",
			"Only Type",
			new SubscriptionPatchRequest
			{
				Type = SubscriptionType.Normal,
			}
		);

		yield return OpenApiExample.Create(
			"onlyStatus",
			"Only Status",
			new SubscriptionPatchRequest
			{
				Status = SubscriptionStatus.Suspended
			}
		);
	}
}

internal class SubscriptionPatchValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		// yield return OpenApiExample.Create(
		// 	"idparametererror",
		// 	"ID Parameter Error",
		// 	"Example response when you provide and ID that isn't Guid",
		// 	ErrorResponse.IdToGuidParameterError
		// );

		return new Dictionary<string, string[]>()
		{
			{ "", ["At least one field must be provided for update."] },
		};
	}
}
