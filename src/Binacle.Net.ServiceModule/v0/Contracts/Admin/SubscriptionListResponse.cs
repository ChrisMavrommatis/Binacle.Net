using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class MinimalSubscription
{
	public required Guid Id { get; set; }
	public required Guid AccountId { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required SubscriptionType Type { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required SubscriptionStatus Status { get; set; }
}

internal class SubscriptionListResponse : PagedResponse<MinimalSubscription>
{
	public static SubscriptionListResponse Create(PagedList<Subscription> subscriptions)
	{
		var response = new SubscriptionListResponse()
		{
			Total = subscriptions.TotalCount,
			Page = subscriptions.PageNumber,
			PageSize = subscriptions.PageSize,
			TotalPages = subscriptions.TotalPages
		};
		foreach (var subscription in subscriptions)
		{
			response.Items.Add(new MinimalSubscription()
			{
				Id = subscription.Id,
				AccountId = subscription.AccountId,
				Type = subscription.Type,
				Status = subscription.Status
			});
		}

		return response;
	}
}

internal class SubscriptionListResponseExample : ISingleOpenApiExamplesProvider<SubscriptionListResponse>
{
	public IOpenApiExample<SubscriptionListResponse> GetExample()
	{
		return OpenApiExample.Create(
			"subscriptionList",
			"Subscription List",
			new SubscriptionListResponse()
			{
				Total = 2,
				Page = 1,
				PageSize = 10,
				TotalPages = 1,
				Items =
				[
					new MinimalSubscription()
					{
						Id = Guid.Parse("526501C7-653C-4430-9808-CF64AAF188FA"),
						AccountId = Guid.Parse("7433FEEC-4863-41DF-BA45-57EB52C3F014"),
						Type = SubscriptionType.Normal,
						Status = SubscriptionStatus.Active,
					},
					new MinimalSubscription()
					{
						Id = Guid.Parse("BA8BD323-97BA-475B-AB91-49C50E02BC06"),
						AccountId = Guid.Parse("E766AC59-30BF-4D0B-A2B9-81434AA9CF15"),
						Type = SubscriptionType.Demo,
						Status = SubscriptionStatus.Suspended,
					},
				]
			}
		);
	}
}

internal class SubscriptionListValidationProblemExample :ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>
		{
			{"Pg", ["'Pg' must be greater than or equal to '1'."]},
			{"Pz", ["'Pz' must be greater than or equal to '1'."]},
		};
	}
}
