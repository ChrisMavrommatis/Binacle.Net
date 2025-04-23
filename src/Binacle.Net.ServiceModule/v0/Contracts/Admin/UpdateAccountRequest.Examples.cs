using OpenApiExamples.Abstractions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;
internal class UpdateAccountRequestExamples : IMultipleOpenApiExamplesProvider<UpdateAccountRequest>
{
	public IEnumerable<IOpenApiExample<UpdateAccountRequest>> GetExamples()
	{
		// TODO Exxamples
		throw new NotImplementedException();
		// yield return OpenApiExample.Create(
		// 	"Promote User",
		// 	"Promote User",
		// 	new UpdateApiUserRequest
		// 	{
		// 		Type = UserType.Admin
		// 	}
		// );
		//
		// yield return OpenApiExample.Create(
		// 	"Demote User",
		// 	"Demote User",
		// 	new UpdateApiUserRequest
		// 	{
		// 		Type = UserType.User
		// 	}
		// );
		//
		// yield return OpenApiExample.Create(
		// 	"Disable User",
		// 	"Disable User",
		// 	new UpdateApiUserRequest
		// 	{
		// 		Status = UserStatus.Inactive
		// 	}
		// );
		//
		// yield return OpenApiExample.Create(
		// 	"Enable User",
		// 	"Enable User",
		// 	new UpdateApiUserRequest
		// 	{
		// 		Status = UserStatus.Active
		// 	}
		// );
		//
		// yield return OpenApiExample.Create(
		// 	"Change Both Status & Type",
		// 	"Change Both Status & Type",
		// 	new UpdateApiUserRequest
		// 	{
		// 		Type = UserType.Admin,
		// 		Status = UserStatus.Active
		// 	}
		// );
	}
}
