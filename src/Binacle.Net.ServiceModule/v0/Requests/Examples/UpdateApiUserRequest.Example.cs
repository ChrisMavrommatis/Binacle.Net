using Binacle.Net.ServiceModule.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Requests.Examples;

internal class UpdateApiUserRequestExample : IMultipleOpenApiExamplesProvider<UpdateApiUserRequest>
{
	public IEnumerable<IOpenApiExample<UpdateApiUserRequest>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"Promote User",
			"Promote User",
			new UpdateApiUserRequest
			{
				Type = UserType.Admin
			}
		);

		yield return OpenApiExample.Create(
			"Demote User",
			"Demote User",
			new UpdateApiUserRequest
			{
				Type = UserType.User
			}
		);

		yield return OpenApiExample.Create(
			"Disable User",
			"Disable User",
			new UpdateApiUserRequest
			{
				Status = UserStatus.Inactive
			}
		);

		yield return OpenApiExample.Create(
			"Enable User",
			"Enable User",
			new UpdateApiUserRequest
			{
				Status = UserStatus.Active
			}
		);

		yield return OpenApiExample.Create(
			"Change Both Status & Type",
			"Change Both Status & Type",
			new UpdateApiUserRequest
			{
				Type = UserType.Admin,
				Status = UserStatus.Active
			}
		);
	}
}
