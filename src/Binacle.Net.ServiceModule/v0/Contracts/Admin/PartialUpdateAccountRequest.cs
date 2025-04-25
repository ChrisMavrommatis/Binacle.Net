using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class PartialUpdateAccountRequest : IWithUsername, IWithEmail, IWithPassword
{
	public string? Username { get; set; }
	public string? Email { get; set; }

	public string? Password { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountStatus>>))]
	public AccountStatus? Status { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountRole>>))]
	public AccountRole? Role { get; set; }

	internal class Validator : AbstractValidator<PartialUpdateAccountRequest>
	{
		public Validator()
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

	internal class Examples : IMultipleOpenApiExamplesProvider<PartialUpdateAccountRequest>
	{
		public IEnumerable<IOpenApiExample<PartialUpdateAccountRequest>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"fullUpdate",
				"Full Update",
				new PartialUpdateAccountRequest
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
				new PartialUpdateAccountRequest
				{
					Username = "user@binacle.net",
				}
			);

			yield return OpenApiExample.Create(
				"passwordOnly",
				"Password Only",
				new PartialUpdateAccountRequest
				{
					Username = "user@binacle.net",
				}
			);

			yield return OpenApiExample.Create(
				"roleAndStatus",
				"Role and Status",
				new PartialUpdateAccountRequest
				{
					Role = AccountRole.User,
					Status = AccountStatus.Suspended
				}
			);
		}
	}

	internal class ErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
	{
		public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"malformedRequest",
				"Malformed Request",
				"Example response when the request is has some syntax errors",
				ErrorResponse.MalformedRequest
			);
			yield return OpenApiExample.Create(
				"idparametererror",
				"ID Parameter Error",
				"Example response when you provide and ID that isn't Guid",
				ErrorResponse.IdToGuidParameterError
			);
			yield return OpenApiExample.Create(
				"validationError1",
				"Validation Error 1",
				"Example response with validation errors",
				ErrorResponse.ValidationError(
				[
					"'Email' is not a valid email address.",
					"The length of 'Password' must be at least 10 characters. You entered 8 characters."
				])
			);

			yield return OpenApiExample.Create(
				"validationError2",
				"Validation Error 2",
				"Example response with validation errors",
				ErrorResponse.ValidationError(
				[
					// TODO: Example
				])
			);
		}
	}
}
