using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

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
	public Guid? SubscriptionId { get; set; }
	public required DateTimeOffset CreatedAtUtc { get; set; }

	public static AccountGetResponse From(Account account)
	{
		return new AccountGetResponse()
		{
			Id = account.Id,
			Username = account.Username,
			Role = account.Role,
			Email = account.Email,
			Status = account.Status,
			PasswordHash = account.Password?.ToString(),
			SecurityStamp = account.SecurityStamp,
			SubscriptionId = account.SubscriptionId,
			CreatedAtUtc = account.CreatedAtUtc
		};
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
				Username = "user@binacle.net",
				Role = AccountRole.User,
				Email = "user@binacle.net",
				Status = AccountStatus.Active,
				PasswordHash = "type::hash::salt",
				SecurityStamp = Guid.Parse("753A88E6-F69B-4362-8B7B-D4B1958C926F"),
				SubscriptionId = Guid.Parse("7C8C06A8-6CD2-441F-AD3C-D25E1AAE1520"),
				CreatedAtUtc = new DateTimeOffset(2025, 1, 11, 14, 30, 53, TimeSpan.Zero),
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
