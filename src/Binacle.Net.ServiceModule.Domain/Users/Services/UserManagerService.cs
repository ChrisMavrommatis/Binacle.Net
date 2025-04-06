using Binacle.Net.ServiceModule.Domain.Users.Data;
using Binacle.Net.ServiceModule.Domain.Users.Entities;
using Binacle.Net.ServiceModule.Domain.Users.Models;
using ChrisMavrommatis.Results.Typed;
using ChrisMavrommatis.Results.Unions;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Binacle.Net.ServiceModule.Services;

public interface IUserManagerService
{
	Task<OneOf<User, Unauthorized>> AuthenticateAsync(AuthenticateUserRequest request, CancellationToken cancellationToken);
	Task<OneOf<User, Conflict, Error>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
	Task<OneOf<Ok, NotFound, Error>> DeleteAsync(DeleteUserRequest request, CancellationToken cancellationToken);
	Task<OneOf<Ok, NotFound, Conflict, Error>> ChangePasswordAsync(ChangeUserPasswordRequest request, CancellationToken cancellationToken);
	Task<OneOf<Ok, NotFound, Error>> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken);
}

internal class UserManagerService : IUserManagerService
{
	private readonly IUserRepository userRepository;
	private readonly ILogger<UserManagerService> logger;
	private readonly TimeProvider timeProvider;
	private static string[] _validGroups = new string[]
	{
		UserGroups.Admins,
		UserGroups.Users
	};

	public UserManagerService(
		IUserRepository userRepository,
		ILogger<UserManagerService> logger,
		TimeProvider timeProvider
		)
	{
		this.userRepository = userRepository;
		this.logger = logger;
		this.timeProvider = timeProvider;
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

		if (!user.IsActive || user.IsDeleted)
		{
			return new Unauthorized();
		}

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

		if (getResult.Is<User>() && !getResult.GetValue<User>().IsDeleted)
		{
			return new Conflict("User already exists");
		}

		byte[] salt = GenerateSalt();
		string hashedPassword = HashPassword(request.Password, salt);

		var user = new User
		{
			Email = request.Email,
			NormalizedEmail = NormalizeEmail(request.Email),
			Group = request.Group,
			HashedPassword = hashedPassword,
			Salt = Convert.ToBase64String(salt),
			CreatedAtUtc = this.timeProvider.GetUtcNow(),
			IsActive = true,
			IsDeleted = false
		};

		if(getResult.Is<User>())
		{
			var updateResult = await this.userRepository.UpdateAsync(user, cancellationToken: cancellationToken);
			if (!updateResult.Is<Ok>())
			{
				return new Error("Could not create user. User was previously deleted");
			}
		}
		else
		{
			var createResult = await this.userRepository.CreateAsync(user, cancellationToken: cancellationToken);
			if (!createResult.Is<Ok>())
			{
				return new Error("Could not create user");
			}
		}
		

		return user;
	}

	public async Task<OneOf<Ok, NotFound, Error>> DeleteAsync(DeleteUserRequest request, CancellationToken cancellationToken = default)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email))
		{
			return new Error("Invalid Email");
		}

		var result = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (!result.Is<User>())
		{
			return new NotFound();
		}

		var user = result.GetValue<User>();

		if (user.IsDeleted)
		{
			return new NotFound();

		}
		user.IsDeleted = true;
		user.DeletedAtUtc = this.timeProvider.GetUtcNow();

		var updateResult = await this.userRepository.UpdateAsync(user, cancellationToken);

		if (!updateResult.Is<Ok>())
		{
			return new Error("Could not soft delete user");
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

		if(user.IsDeleted)
		{
			return new NotFound();
		}

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
			return new Error("Could not change user's password");
		}

		return new Ok();
	}

	public async Task<OneOf<Ok, NotFound, Error>> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken)
	{
		if (request is null || string.IsNullOrWhiteSpace(request.Email))
		{
			return new Error("Invalid Credentials");
		}

		if (string.IsNullOrEmpty(request.Group) && !request.IsActive.HasValue)
		{
			return new Error("Invalid Request");
		}

		if (!string.IsNullOrEmpty(request.Group) && !_validGroups.Contains(request.Group))
		{
			return new Error("Invalid Group");
		}

		var getResult = await this.userRepository.GetAsync(request.Email, cancellationToken);

		if (!getResult.Is<User>())
		{
			return new NotFound();
		}

		var user = getResult.GetValue<User>();
		
		if (user.IsDeleted)
		{
			return new NotFound();
		}

		if (!string.IsNullOrEmpty(request.Group))
		{
			user.Group = request.Group!;
		}

		if (request.IsActive.HasValue)
		{
			user.IsActive = request.IsActive.Value;
		}

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

	private static string NormalizeEmail(string email)
	{
		return email.Trim().ToLowerInvariant();
	}
}
