using System.Text.Json.Serialization;
using Binacle.Net.Kernel.OpenApi.Helpers;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class AccountPatchRequest : IWithUsername, IWithEmail, IWithPassword
{
	public string? Username { get; set; }
	public string? Email { get; set; }
	public string? Password { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public AccountStatus? Status { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public AccountRole? Role { get; set; }
}

internal class AccountPatchRequestValidator : AbstractValidator<AccountPatchRequest>
{
	public AccountPatchRequestValidator()
	{
		When(x => !string.IsNullOrEmpty(x.Email), () =>
		{
			RuleFor(x => x)
				.SetValidator(new EmailValidator());
		});

		When(x => !string.IsNullOrEmpty(x.Password), () =>
		{
			RuleFor(x => x)
				.SetValidator(new PasswordValidator());
		});

		When(x => !string.IsNullOrEmpty(x.Username), () =>
		{
			RuleFor(x => x)
				.SetValidator(new UsernameValidator());
		});

		RuleFor(x => x)
			.Must(x =>
				!string.IsNullOrEmpty(x.Email)
				|| !string.IsNullOrEmpty(x.Password)
				|| !string.IsNullOrEmpty(x.Username)
				|| x.Status.HasValue
				|| x.Role.HasValue
			).WithMessage(
				"At least one field must be provided for update."
			);
	}
}

internal class AccountPatchRequestExamples : IMultipleOpenApiExamplesProvider<AccountPatchRequest>
{
	public IEnumerable<IOpenApiExample<AccountPatchRequest>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullUpdate",
			"Full Update",
			new AccountPatchRequest
			{
				Username = "user@binacle.net",
				Email = "user@binacle.net",
				Password = "userspassword",
				Role = AccountRole.User,
				Status = AccountStatus.Active
			}
		);

		yield return OpenApiExample.Create(
			"usernameOnly",
			"Username Only",
			new AccountPatchRequest
			{
				Username = "user@binacle.net",
			}
		);

		yield return OpenApiExample.Create(
			"passwordOnly",
			"Password Only",
			new AccountPatchRequest
			{
				Username = "user@binacle.net",
			}
		);

		yield return OpenApiExample.Create(
			"roleAndStatus",
			"Role and Status",
			new AccountPatchRequest
			{
				Role = AccountRole.User,
				Status = AccountStatus.Suspended
			}
		);
	}
}

internal class AccountPatchValidationProblemExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiValidationProblemExample.Create(
			"validationProblem",
			"Validation Problem",
			"Example response with validation errors",
			new Dictionary<string, string[]>()
			{
				{ "Email", ["'Email' is not a valid email address."] },
				{ "Password", ["The length of 'Password' must be at least 10 characters. You entered 8 characters."] }
			}
		);

		yield return OpenApiValidationProblemExample.Create(
			"invalidId",
			"Invalid Id",
			"Example response when you provide and ID that isn't Guid",
			new Dictionary<string, string[]>()
			{
				{ "Id", [ErrorMessage.IdMustBeGuid] },
			}
		);

		yield return OpenApiValidationProblemExample.Create(
			"validationProblem2",
			"Validation Problem 2",
			"Example response with validation errors",
			new Dictionary<string, string[]>()
			{
				{ "", ["At least one field must be provided for update."] },
			}
		);
	}
}
