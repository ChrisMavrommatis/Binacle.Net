using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Models;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;


internal class UpdateAccountRequest
{
	public string Email { get; set; }
	
	public string Password { get; set; }
	
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountStatus>>))]
	public AccountStatus? Status { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountRole>>))]
	public AccountRole? Role { get; set; }
}

