using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

// No password hash and no security stamp on purpose. One account fetched by id is a different thing from a
// page of them - the same fields here put the whole credential table on the wire in one call.
internal class AccountListItem
{
	public required Guid Id { get; set; }
	public required string Username { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required AccountRole Role { get; set; }

	public required string Email { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required AccountStatus Status { get; set; }

	public Guid? SubscriptionId { get; set; }
	public required DateTimeOffset CreatedAtUtc { get; set; }
	public required bool IsDeleted { get; set; }
	public DateTimeOffset? DeletedAtUtc { get; set; }

	public static AccountListItem From(Account account)
	{
		return new AccountListItem()
		{
			Id = account.Id,
			Username = account.Username,
			Role = account.Role,
			Email = account.Email,
			Status = account.Status,
			SubscriptionId = account.SubscriptionId,
			CreatedAtUtc = account.CreatedAtUtc,
			IsDeleted = account.IsDeleted,
			DeletedAtUtc = account.DeletedAtUtc
		};
	}
}

internal class AccountListResponseExample : ISingleOpenApiExamplesProvider<PagedResponse<AccountListItem>>
{
	public IOpenApiExample<PagedResponse<AccountListItem>> GetExample()
	{
		return OpenApiExample.Create(
			"accountList",
			"Account List",
			new PagedResponse<AccountListItem>()
			{
				Total = 2,
				Page = 1,
				PageSize = 50,
				TotalPages = 1,
				Items =
				[
					new AccountListItem()
					{
						Id = Guid.Parse("7433FEEC-4863-41DF-BA45-57EB52C3F014"),
						Username = "user@example.binacle.net",
						Role = AccountRole.User,
						Email = "user@example.binacle.net",
						Status = AccountStatus.Active,
						SubscriptionId = Guid.Parse("526501C7-653C-4430-9808-CF64AAF188FA"),
						CreatedAtUtc = new DateTimeOffset(2025, 1, 11, 14, 30, 53, TimeSpan.Zero),
						IsDeleted = false
					},
					new AccountListItem()
					{
						Id = Guid.Parse("A0C0E0A6-9A4E-4E5D-9E9B-9C4E9F4E1B22"),
						Username = "admin@example.binacle.net",
						Role = AccountRole.Admin,
						Email = "admin@example.binacle.net",
						Status = AccountStatus.Active,
						CreatedAtUtc = new DateTimeOffset(2025, 1, 10, 9, 12, 4, TimeSpan.Zero),
						IsDeleted = false
					}
				]
			}
		);
	}
}
