using Binacle.Net.Api.ServiceModule.Data.Entities;
using Binacle.Net.Api.ServiceModule.Data.Repositories;
using Binacle.Net.Api.ServiceModule.Data.Schemas;
using Binacle.Net.Api.ServiceModule.Models;
using System.Security.Cryptography;
using System.Text;

namespace Binacle.Net.Api.ServiceModule.Services;

internal interface IAuthService
{
	Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken);
	Task<CreateUserResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
}

internal class AuthService : IAuthService
{
	private readonly IUserRepository userRepository;

	public AuthService(
		IUserRepository userRepository
		)
	{
		this.userRepository = userRepository;
	}

	public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new AuthenticationResult(false, AuthenticationFailedResultReason.InvalidCredentials);
		}
		var user = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (user is null)
		{
			return new AuthenticationResult(false, AuthenticationFailedResultReason.InvalidCredentials);
		}

		var hashedPassword = HashPassword(request.Password, Convert.FromBase64String(user.Salt));

		if (hashedPassword != user.HashedPassword)
		{
			return new AuthenticationResult(false, AuthenticationFailedResultReason.InvalidCredentials);
		}

		return new AuthenticationResult(true, User: user);
	}

	public async Task<CreateUserResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new CreateUserResult(false, CreateUserFailedResultReason.InvalidCredentials);
		}

		var user = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (user is null)
		{
			return new CreateUserResult(false, CreateUserFailedResultReason.AlreadyExists);
		}

		byte[] salt = GenerateSalt();
		string hashedPassword = HashPassword(request.Password, salt);

		user = new UserEntity(request.Email, UserGroups.Users)
		{
			Salt = Convert.ToBase64String(salt),
			HashedPassword = hashedPassword
		};

		var success = await this.userRepository.CreateAsync(user, cancellationToken: cancellationToken);
		if (!success)
		{
			return new CreateUserResult(false, CreateUserFailedResultReason.Unknown);
		}

		return new CreateUserResult(true);
	}

	private static byte[] GenerateSalt()
	{
		return RandomNumberGenerator.GetBytes(16);
	}

	private static string HashPassword(string password, byte[] salt)
	{
		using (var sha256 = SHA256.Create())
		{
			byte[] combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
			byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
			return Convert.ToBase64String(hashedBytes);
		}
	}
}
