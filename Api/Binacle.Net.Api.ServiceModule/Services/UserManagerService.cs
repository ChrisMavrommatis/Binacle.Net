using Binacle.Net.Api.ServiceModule.Data.Entities;
using Binacle.Net.Api.ServiceModule.Data.Repositories;
using Binacle.Net.Api.ServiceModule.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Binacle.Net.Api.ServiceModule.Services;

internal interface IUserManagerService
{
	Task<UserActionResult> AuthenticateAsync(UserActionRequest request, CancellationToken cancellationToken);
	Task<UserActionResult> CreateAsync(UserActionRequest request, CancellationToken cancellationToken);
	Task<UserActionResult> DeleteAsync(UserActionRequest request, CancellationToken cancellationToken);
	Task<UserActionResult> ChangePasswordAsync(UserActionRequest request, CancellationToken cancellationToken);
}

internal class UserManagerService : IUserManagerService
{
	private readonly IUserRepository userRepository;
	private readonly ILogger<UserManagerService> logger;

	public UserManagerService(
		IUserRepository userRepository,
		ILogger<UserManagerService> logger
		)
	{
		this.userRepository = userRepository;
		this.logger = logger;
	}

	public async Task<UserActionResult> AuthenticateAsync(UserActionRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new UserActionResult(false, Message: "Invalid Credentials", ResultType: UserActionResultType.Unauthorized);
		}

		var result = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (!result.Success)
		{
			return new UserActionResult(false, Message: "Invalid Credentials", ResultType: UserActionResultType.Unauthorized);
		}

		var user = result.Value!;

		var hashedPassword = HashPassword(request.Password, Convert.FromBase64String(user.Salt));

		if (hashedPassword != user.HashedPassword)
		{
			return new UserActionResult(false, Message: "Invalid Credentials", ResultType: UserActionResultType.Unauthorized);
		}

		return new UserActionResult(true, User: user);
	}

	public async Task<UserActionResult> CreateAsync(UserActionRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new UserActionResult(false, Message: "Invalid Credentials", ResultType: UserActionResultType.MalformedRequest);
		}

		var getResult = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (getResult.Success)
		{
			return new UserActionResult(false, Message: "User already exists", ResultType: UserActionResultType.Conflict);
		}

		byte[] salt = GenerateSalt();
		string hashedPassword = HashPassword(request.Password, salt);

		var user = new UserEntity(request.Email, UserGroups.Users)
		{
			Salt = Convert.ToBase64String(salt),
			HashedPassword = hashedPassword
		};

		var createResult = await this.userRepository.CreateAsync(user, cancellationToken: cancellationToken);
		if (!createResult.Success)
		{
			return new UserActionResult(false, Message: "Could not create user", ResultType: UserActionResultType.Unspecified);
		}

		return new UserActionResult(true, User: user);
	}

	public async Task<UserActionResult> DeleteAsync(UserActionRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email))
		{
			return new UserActionResult(false, Message: "Invalid Email", ResultType: UserActionResultType.MalformedRequest);
		}
		var result = await this.userRepository.DeleteAsync(request.Email, cancellationToken);

		if (!result.Success)
		{
			return new UserActionResult(false, Message: "User doesn't exist", ResultType: UserActionResultType.NotFound);
		}

		return new UserActionResult(true);
	}

	public async Task<UserActionResult> ChangePasswordAsync(UserActionRequest request, CancellationToken cancellationToken)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new UserActionResult(false, Message: "Invalid Credentials", ResultType: UserActionResultType.MalformedRequest);
		}

		var getResult = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (!getResult.Success)
		{
			return new UserActionResult(false, Message: "User doesn't exist", ResultType: UserActionResultType.NotFound);
		}

		var user = getResult.Value!;
		var hashedPassword = HashPassword(request.Password, Convert.FromBase64String(user.Salt));

		if (hashedPassword == user.HashedPassword)
		{
			return new UserActionResult(false, Message: "New password is the same as the old", ResultType: UserActionResultType.Conflict);
		}

		byte[] salt = GenerateSalt();
		string newHashedPassword = HashPassword(request.Password, salt);

		user.Salt = Convert.ToBase64String(salt);
		user.HashedPassword = newHashedPassword;

		var updateResult = await this.userRepository.UpdateAsync(user, cancellationToken);

		if (!updateResult.Success)
		{
			return new UserActionResult(false, Message: "Could not update user", ResultType: UserActionResultType.Unspecified);
		}

		return new UserActionResult(true, User: user);
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
