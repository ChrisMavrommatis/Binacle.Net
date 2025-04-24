using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class PartialUpdateAccountRequestExamples : IMultipleOpenApiExamplesProvider<PartialUpdateAccountRequest>
{
	public IEnumerable<IOpenApiExample<PartialUpdateAccountRequest>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullUpdate",
			"Full Update",
			new PartialUpdateAccountRequest
			{
				Username = "user@binacle.net",
				Email = "user@binacle.net",
				Password = "userspassword",
				Role = AccountRole.User,
				Status = AccountStatus.Active
			}
		);
		
		yield return OpenApiExample.Create(
			"usernameOnly",
			"Username Only",
			new PartialUpdateAccountRequest
			{
				Username = "user@binacle.net",
			}
		);
		
		yield return OpenApiExample.Create(
			"passwordOnly",
			"Password Only",
			new PartialUpdateAccountRequest
			{
				Username = "user@binacle.net",
			}
		);
		
		yield return OpenApiExample.Create(
			"roleAndStatus",
			"Role and Status",
			new PartialUpdateAccountRequest
			{
				Role = AccountRole.User,
				Status = AccountStatus.Suspended
			}
		);
	}
}
