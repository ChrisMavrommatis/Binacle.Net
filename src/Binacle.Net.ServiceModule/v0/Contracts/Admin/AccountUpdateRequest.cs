using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class AccountUpdateRequest : IWithUsername, IWithEmail, IWithPassword
{
	[Required] 
	public string Username { get; set; } = null!;

	[Required] 
	public string Email { get; set; } = null!;

	[Required] 
	public string Password { get; set; } = null!;

	[Required]
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountStatus>>))]
	public AccountStatus? Status { get; set; }

	[Required]
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountRole>>))]
	public AccountRole? Role { get; set; }
}

internal class AccountUpdateRequestValidator : AbstractValidator<AccountUpdateRequest>
{
	public AccountUpdateRequestValidator()
	{
		Include(x => new UsernameValidator());
		Include(x => new EmailValidator());
		Include(x => new PasswordValidator());

		RuleFor(x => x.Status)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<AccountStatus>(nameof(AccountUpdateRequest.Status)));

		RuleFor(x => x.Role)
			.NotNull()
			.WithMessage(ErrorMessage.RequiredEnumValues<AccountRole>(nameof(AccountUpdateRequest.Role)));
	}
}

internal class AccountUpdateRequestExample : ISingleOpenApiExamplesProvider<AccountUpdateRequest>
{
	public IOpenApiExample<AccountUpdateRequest> GetExample()
	{
		return OpenApiExample.Create(
			"updateAccountRequest",
			"Update Account Request",
			new AccountUpdateRequest
			{
				Username = "user@binacle.net",
				Email = "user@binacle.net",
				Password = "userspassword",
				Role = AccountRole.User,
				Status = AccountStatus.Active
			}
		);
	}
}

internal class AccountUpdateValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>()
		{
			{ "Email", ["'Email' is not a valid email address."] },
			{ "Password", ["The length of 'Password' must be at least 10 characters. You entered 8 characters."] },
			{ "Role", [ErrorMessage.RequiredEnumValues<AccountRole>(nameof(AccountUpdateRequest.Role))] },
			{ "Status", [ErrorMessage.RequiredEnumValues<AccountStatus>(nameof(AccountUpdateRequest.Status))] }
		};
	}
}
