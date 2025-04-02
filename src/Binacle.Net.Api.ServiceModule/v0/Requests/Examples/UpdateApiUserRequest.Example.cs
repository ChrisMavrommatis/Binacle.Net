// using ChrisMavrommatis.SwaggerExamples;
// using ChrisMavrommatis.SwaggerExamples.Abstractions;
//
// namespace Binacle.Net.Api.ServiceModule.v0.Requests.Examples;
//
// internal class UpdateApiUserRequestExample : MultipleSwaggerExamplesProvider<UpdateApiUserRequest>
// {
// 	public override IEnumerable<ISwaggerExample<UpdateApiUserRequest>> GetExamples()
// 	{
// 		yield return SwaggerExample.Create("Promote User", "Promote User", new UpdateApiUserRequest
// 		{
// 			Type = Models.UserType.Admin
// 		});
//
// 		yield return SwaggerExample.Create("Demote User", "Demote User", new UpdateApiUserRequest
// 		{
// 			Type = Models.UserType.User
// 		});
//
// 		yield return SwaggerExample.Create("Disable User", "Disable User", new UpdateApiUserRequest
// 		{
// 			Status = Models.UserStatus.Inactive
// 		});
//
// 		yield return SwaggerExample.Create("Enable User", "Enable User", new UpdateApiUserRequest
// 		{
// 			Status = Models.UserStatus.Active
// 		});
//
// 		yield return SwaggerExample.Create("Change Both Status & Type", "Change Both Status & Type", new UpdateApiUserRequest
// 		{
// 			Type = Models.UserType.Admin,
// 			Status = Models.UserStatus.Active
// 		});
// 	}
// }
