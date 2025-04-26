using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

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
			PasswordHash = account.Password?.ToString(),
			SecurityStamp = account.SecurityStamp,
			SubscriptionId = account.SubscriptionId,
			CreatedAtUtc = account.CreatedAtUtc
		};
	}
	
	
	internal class Example : ISingleOpenApiExamplesProvider<GetAccountResponse>
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
	internal class ErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
	{
		public IOpenApiExample<ErrorResponse> GetExample()
		{
			return OpenApiExample.Create(
				"idparametererror",
				"ID Parameter Error",
				ErrorResponse.IdToGuidParameterError
			);
		}
	}
}
