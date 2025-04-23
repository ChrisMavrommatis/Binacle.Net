using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class GetAccountResponse
{
	public required string Username { get; set; }
	
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public AccountRole Role { get; set; }
	public required string Email { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public AccountStatus Status { get; set; }
	public string? PasswordHash { get; set; }
	public Guid SecurityStamp { get;  set; }
	public Guid? SubscriptionId { get; set; }
	public DateTimeOffset CreatedAtUtc { get; set; }
	
	public static GetAccountResponse From(Account account)
	{
		return new GetAccountResponse()
		{
			Username = account.Username,
			Role = account.Role,
			Email = account.Email,
			Status = account.Status,
			PasswordHash = account.PasswordHash,
			SecurityStamp = account.SecurityStamp,
			SubscriptionId = account.SubscriptionId,
			CreatedAtUtc = account.CreatedAtUtc
		};
	}
}
