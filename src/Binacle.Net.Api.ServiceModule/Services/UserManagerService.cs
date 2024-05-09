using Binacle.Net.Api.ServiceModule.Domain.Users.Data;
using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Domain.Users.Models;
using ChrisMavrommatis.Results.Typed;
using ChrisMavrommatis.Results.Unions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Binacle.Net.Api.ServiceModule.Services;

internal interface IUserManagerService
{
	Task<OneOf<User, Unauthorized>> AuthenticateAsync(AuthenticateUserRequest request, CancellationToken cancellationToken);
	Task<OneOf<User, Conflict, Error>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
	Task<OneOf<Ok, NotFound, Error>> DeleteAsync(DeleteUserRequest request, CancellationToken cancellationToken);
	Task<OneOf<Ok, NotFound, Conflict, Error>> ChangePasswordAsync(ChangeUserPasswordRequest request, CancellationToken cancellationToken);
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

	public async Task<OneOf<User, Unauthorized>> AuthenticateAsync(AuthenticateUserRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new Unauthorized();
		}

		var result = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (!result.Is<User>())
		{
			return new Unauthorized();
		}


		var user = result.GetValue<User>();

		var hashedPassword = HashPassword(request.Password, Convert.FromBase64String(user.Salt));

		if (hashedPassword != user.HashedPassword)
		{
			return new Unauthorized();
		}

		return user;
	}

	public async Task<OneOf<User, Conflict, Error>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
		{
			return new Error("Invalid Credentials");
		}

		var getResult = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (getResult.Is<User>())
		{
			return new Error("User already exists");
		}

		byte[] salt = GenerateSalt();
		string hashedPassword = HashPassword(request.Password, salt);

		var user = new User
		{
			Email = request.Email,
			Group = UserGroups.Users,
			Salt = Convert.ToBase64String(salt),
			HashedPassword = hashedPassword
		};

		var createResult = await this.userRepository.CreateAsync(user, cancellationToken: cancellationToken);
		if (!createResult.Is<Ok>())
		{
			return new Error("Could not create user");
		}

		return user;
	}

	public async Task<OneOf<Ok, NotFound, Error>> DeleteAsync(DeleteUserRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email))
		{
			return new Error("Invalid Email");
		}
		var result = await this.userRepository.DeleteAsync(request.Email, cancellationToken);

		if (!result.Is<Ok>())
		{
			return new NotFound();
		}

		return new Ok();
	}

	public async Task<OneOf<Ok, NotFound, Conflict, Error>> ChangePasswordAsync(ChangeUserPasswordRequest request, CancellationToken cancellationToken)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
		{
			return new Error("Invalid Credentials");
		}

		var getResult = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (!getResult.Is<User>())
		{
			return new NotFound();
		}

		var user = getResult.GetValue<User>();
		var newHashedPassword = HashPassword(request.NewPassword, Convert.FromBase64String(user.Salt));

		if (newHashedPassword == user.HashedPassword)
		{
			return new Conflict("New password is the same as the old");
		}

		byte[] salt = GenerateSalt();
		string newHashedPasswordWithDifferentSalt = HashPassword(request.NewPassword, salt);

		user.Salt = Convert.ToBase64String(salt);
		user.HashedPassword = newHashedPasswordWithDifferentSalt;

		var updateResult = await this.userRepository.UpdateAsync(user, cancellationToken);

		if (!updateResult.Is<Ok>())
		{
			return new Error("Could not update user");
		}

		return new Ok();
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
