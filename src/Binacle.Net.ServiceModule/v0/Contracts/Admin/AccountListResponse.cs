using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class MinimalAccount
{
	public required Guid Id { get; set; }
	public required string Username { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required AccountRole Role { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required AccountStatus Status { get; set; }
}

internal class AccountListResponse : PagedResponse<MinimalAccount>
{
	public static AccountListResponse Create(PagedList<Account> pagedResult)
	{
		var response = new AccountListResponse()
		{
			Total = pagedResult.TotalCount,
			Page = pagedResult.PageNumber,
			PageSize = pagedResult.PageSize,
			TotalPages = pagedResult.TotalPages,
		};

		foreach (var account in pagedResult)
		{
			response.Items.Add(
				new MinimalAccount
				{
					Id = account.Id,
					Username = account.Username,
					Role = account.Role,
					Status = account.Status
				}
			);
		}

		return response;
	}
}

internal class AccountListResponseExample : ISingleOpenApiExamplesProvider<AccountListResponse>
{
	public IOpenApiExample<AccountListResponse> GetExample()
	{
		return OpenApiExample.Create(
			"accountList",
			"Account List",
			new AccountListResponse()
			{
				Total = 2,
				Page = 1,
				PageSize = 10,
				TotalPages = 1,
				Items = [
					new MinimalAccount()
					{
						Id = Guid.Parse("7433FEEC-4863-41DF-BA45-57EB52C3F014"),
						Username = "user@binacle.net",
						Role = AccountRole.User,
						Status = AccountStatus.Active
					},
					new MinimalAccount()
					{
						Id = Guid.Parse("E766AC59-30BF-4D0B-A2B9-81434AA9CF15"),
						Username = "user2@binacle.net",
						Role = AccountRole.User,
						Status = AccountStatus.Suspended
					}
				]
			}
		);
	}
}

internal class AccountListValidationProblemExample : ValidationProblemResponseExample
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
