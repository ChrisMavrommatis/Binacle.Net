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

internal class UpdateAccountRequest : IWithUsername, IWithEmail, IWithPassword
{
	public string Username { get; set; }
	public string Email { get; set; }
	
	public string Password { get; set; }
	
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountStatus>>))]
	public AccountStatus? Status { get; set; }

	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<AccountRole>>))]
	public AccountRole? Role { get; set; }
	
	
	internal class Validator : AbstractValidator<UpdateAccountRequest>
	{
		public Validator()
		{
			Include(x => new UsernameValidator());
			Include(x => new EmailValidator());
			Include(x => new PasswordValidator());
		
			var accountStatusValues = Enum.GetValues<AccountStatus>();
		
			RuleFor(x => x.Status)
				.NotNull()
				.WithMessage($"'{nameof(UpdateAccountRequest.Status)}' is required and must be one of the following values: {string.Join(", ", accountStatusValues)}");

			var accountRoleValues = Enum.GetValues<AccountRole>();

			RuleFor(x => x.Role)
				.NotNull()
				.WithMessage($"'{nameof(UpdateAccountRequest.Role)}' is required and must be one of the following values: {string.Join(", ", accountRoleValues)}");

		}
	}


	internal class Example : ISingleOpenApiExamplesProvider<UpdateAccountRequest>
	{
		public IOpenApiExample<UpdateAccountRequest> GetExample()
		{
			return OpenApiExample.Create(
				"updateAccountRequest",
				"Update Account Request",
				new UpdateAccountRequest
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
