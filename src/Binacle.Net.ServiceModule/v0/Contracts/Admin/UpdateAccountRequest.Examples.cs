using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;
internal class UpdateAccountRequestExample : ISingleOpenApiExamplesProvider<UpdateAccountRequest>
{
	public IOpenApiExample<UpdateAccountRequest> GetExample()
	{
		return OpenApiExample.Create(
			"updateAccountRequest",
			"Update Account Request",
			new UpdateAccountRequest
			{
				Username = "user@binacle.net",
				Email = "user@binacle.net",
				Password = "userspassword",
				Role = AccountRole.User,
				Status = AccountStatus.Active
			}
		);
	}
}
