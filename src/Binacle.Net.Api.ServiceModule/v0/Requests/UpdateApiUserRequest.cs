using Binacle.Net.Api.Kernel.Serialization;
using Binacle.Net.Api.ServiceModule.Domain.Models;
using Binacle.Net.Api.ServiceModule.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.ServiceModule.v0.Requests;

internal class UpdateApiUserRequestWithBody: IWithEmail
{
	[FromRoute]
	public string Email { get; set; }

	[FromBody]
	public UpdateApiUserRequest Body { get; set; }
}

internal class UpdateApiUserRequest
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<UserType>>))]
	public UserType? Type { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<UserStatus>>))]
	public UserStatus? Status { get; set; }
}
