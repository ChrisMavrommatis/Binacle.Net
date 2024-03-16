using Azure.Data.Tables;
using Binacle.Net.Api.Users.Data.Entities;
using Binacle.Net.Api.Users.Data.Schemas;
using Binacle.Net.Api.Users.Helpers;
using Binacle.Net.Api.Users.Models;

namespace Binacle.Net.Api.Users.Data.Services;

internal interface IAuthService
{
	Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken);
	Task<CreateUserResult> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
}

internal class AuthService : IAuthService
{
	private readonly TableServiceClient tableServiceClient;

	public AuthService(
		TableServiceClient tableServiceClient
		)
	{
		this.tableServiceClient = tableServiceClient;
	}

	public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new AuthenticationResult(false, AuthenticationFailedResultReason.InvalidCredentials);
		}

		var tableClient = tableServiceClient.GetTableClient(TableNames.Users);

		var user = await tableClient.GetEntityIfExistsAsync<UserEntity>(UserGroups.Users, request.Email, cancellationToken: cancellationToken);

		if (!user.HasValue)
		{
			return new AuthenticationResult(false, AuthenticationFailedResultReason.InvalidCredentials);
		}

		var hashedPassword = CryptoHelper.HashPassword(request.Password, Convert.FromBase64String(user.Value!.Salt));

		if (hashedPassword != user.Value.HashedPassword)
		{
			return new AuthenticationResult(false, AuthenticationFailedResultReason.InvalidCredentials);
		}

		return new AuthenticationResult(true, User: user.Value);
	}

	public async Task<CreateUserResult> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new CreateUserResult(false, CreateUserFailedResultReason.InvalidCredentials);
		}

		await tableServiceClient.CreateTableIfNotExistsAsync(TableNames.Users);

		var tableClient = tableServiceClient.GetTableClient(TableNames.Users);

		var response = await tableClient.GetEntityIfExistsAsync<UserEntity>(UserGroups.Users, request.Email, cancellationToken: cancellationToken);

		if (response.HasValue)
		{
			return new CreateUserResult(false, CreateUserFailedResultReason.AlreadyExists);
		}

		byte[] salt = CryptoHelper.GenerateSalt();
		string hashedPassword = CryptoHelper.HashPassword(request.Password, salt);

		var user = new UserEntity(request.Email, UserGroups.Users)
		{
			Salt = Convert.ToBase64String(salt),
			HashedPassword = hashedPassword
		};
		var result = await tableClient.AddEntityAsync(user, cancellationToken: cancellationToken);
		if (result.IsError)
		{
			return new CreateUserResult(false, CreateUserFailedResultReason.Unknown);
		}

		return new CreateUserResult(true);
	}
}
