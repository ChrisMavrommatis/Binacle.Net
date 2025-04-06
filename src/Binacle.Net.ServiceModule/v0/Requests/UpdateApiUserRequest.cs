using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Models;
using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Models;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Requests;

internal class UpdateApiUserRequestWithBody: IWithEmail
{
	[FromRoute]
	public string Email { get; set; }

	[FromBody]
	public UpdateApiUserRequest? Body { get; set; }
}

internal class UpdateApiUserRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<UserType>>))]
	public UserType? Type { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<UserStatus>>))]
	public UserStatus? Status { get; set; }
}
