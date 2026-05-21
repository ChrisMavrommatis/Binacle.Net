using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class AccountSubscription
{
	public required Guid Id { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required SubscriptionType Type { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required SubscriptionStatus Status { get; set; }
	public required DateTimeOffset CreatedAtUtc { get; set; }
	public required bool IsDeleted { get; set; }
	public DateTimeOffset? DeletedAtUtc { get; set; }
}

internal class AccountGetResponse
{
	public required Guid Id { get; set; }
	public required string Username { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required AccountRole Role { get; set; }

	public required string Email { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required AccountStatus Status { get; set; }

	public string? PasswordHash { get; set; }
	public required Guid SecurityStamp { get; set; }
	public AccountSubscription? Subscription { get; set; }
	public required DateTimeOffset CreatedAtUtc { get; set; }
	public required bool IsDeleted { get; set; }
	public DateTimeOffset? DeletedAtUtc { get; set; }

	public static AccountGetResponse From(Account account, Subscription? subscription = null)
	{
		var response = new AccountGetResponse()
		{
			Id = account.Id,
			Username = account.Username,
			Role = account.Role,
			Email = account.Email,
			Status = account.Status,
			PasswordHash = account.Password?.ToString(),
			SecurityStamp = account.SecurityStamp,
			CreatedAtUtc = account.CreatedAtUtc,
			IsDeleted = account.IsDeleted,
			DeletedAtUtc = account.DeletedAtUtc
		};
		
		if (subscription is not null)
		{
			response.Subscription = new AccountSubscription()
			{
				Id = subscription.Id,
				Type = subscription.Type,
				Status = subscription.Status,
				CreatedAtUtc = subscription.CreatedAtUtc,
				IsDeleted = subscription.IsDeleted,
				DeletedAtUtc = subscription.DeletedAtUtc
			};
		}

		return response;
	}
}

internal class AccountGetResponseExample : ISingleOpenApiExamplesProvider<AccountGetResponse>
{
	public IOpenApiExample<AccountGetResponse> GetExample()
	{
		return OpenApiExample.Create(
			"accountGet",
			"Account Get",
			new AccountGetResponse()
			{
				Id = Guid.Parse("7433FEEC-4863-41DF-BA45-57EB52C3F014"),
				Username = "user@example.binacle.net",
				Role = AccountRole.User,
				Email = "user@example.binacle.net",
				Status = AccountStatus.Active,
				PasswordHash = "type::hash::salt",
				SecurityStamp = Guid.Parse("753A88E6-F69B-4362-8B7B-D4B1958C926F"),
				CreatedAtUtc = new DateTimeOffset(2025, 1, 11, 14, 30, 53, TimeSpan.Zero),
				IsDeleted = false,
				Subscription = new AccountSubscription()
				{
					Id = Guid.Parse("526501C7-653C-4430-9808-CF64AAF188FA"),
					Type = SubscriptionType.Normal,
					Status = SubscriptionStatus.Active,
					CreatedAtUtc = new DateTimeOffset(2025, 1, 11, 14, 35, 23, TimeSpan.Zero),
					IsDeleted = false,
				}
			}
		);
	}
}

internal class AccountGetValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>()
		{
			{ "Id", [ErrorMessage.IdMustBeGuid] }
		};
	}
}
