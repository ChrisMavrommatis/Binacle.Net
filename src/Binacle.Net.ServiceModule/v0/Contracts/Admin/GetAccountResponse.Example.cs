using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class GetAccountResponseExample : ISingleOpenApiExamplesProvider<GetAccountResponse>
{
	public IOpenApiExample<GetAccountResponse> GetExample()
	{
		return OpenApiExample.Create(
			"getAccountResponse",
			"Get Account Response",
			new GetAccountResponse()
			{
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
