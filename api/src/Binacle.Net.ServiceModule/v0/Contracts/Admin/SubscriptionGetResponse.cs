using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

// Also the row type for the subscription list - a subscription is small enough that the list and the single
// read return the same shape.
internal class SubscriptionGetResponse
{
	public required Guid Id { get; set; }
	public required Guid AccountId { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required SubscriptionType Type { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required SubscriptionStatus Status { get; set; }

	public required DateTimeOffset CreatedAtUtc { get; set; }
	public required bool IsDeleted { get; set; }
	public DateTimeOffset? DeletedAtUtc { get; set; }

	public static SubscriptionGetResponse From(Subscription subscription)
	{
		return new SubscriptionGetResponse()
		{
			Id = subscription.Id,
			AccountId = subscription.AccountId,
			Type = subscription.Type,
			Status = subscription.Status,
			CreatedAtUtc = subscription.CreatedAtUtc,
			IsDeleted = subscription.IsDeleted,
			DeletedAtUtc = subscription.DeletedAtUtc
		};
	}
}

internal class SubscriptionGetResponseExample : ISingleOpenApiExamplesProvider<SubscriptionGetResponse>
{
	public IOpenApiExample<SubscriptionGetResponse> GetExample()
	{
		return OpenApiExample.Create(
			"subscriptionGet",
			"Subscription Get",
			Sample
		);
	}

	internal static SubscriptionGetResponse Sample => new()
	{
		Id = Guid.Parse("526501C7-653C-4430-9808-CF64AAF188FA"),
		AccountId = Guid.Parse("7433FEEC-4863-41DF-BA45-57EB52C3F014"),
		Type = SubscriptionType.Normal,
		Status = SubscriptionStatus.Active,
		CreatedAtUtc = new DateTimeOffset(2025, 1, 11, 14, 35, 23, TimeSpan.Zero),
		IsDeleted = false
	};
}

internal class SubscriptionListResponseExample : ISingleOpenApiExamplesProvider<PagedResponse<SubscriptionGetResponse>>
{
	public IOpenApiExample<PagedResponse<SubscriptionGetResponse>> GetExample()
	{
		return OpenApiExample.Create(
			"subscriptionList",
			"Subscription List",
			new PagedResponse<SubscriptionGetResponse>()
			{
				Total = 1,
				Page = 1,
				PageSize = 50,
				TotalPages = 1,
				Items = [SubscriptionGetResponseExample.Sample]
			}
		);
	}
}

internal class SubscriptionGetValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>()
		{
			{ "Id", [ErrorMessage.IdMustBeGuid] }
		};
	}
}
