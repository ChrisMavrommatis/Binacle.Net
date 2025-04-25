using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class AuthErrorResponse
{
	public required string Message { get; set; }
	public string[]? Errors { get; set; }
	internal static AuthErrorResponse Create(string message, string[]? errors = null)
	{
		return new AuthErrorResponse
		{
			Message = message,
			Errors = errors
		};
	}

	internal static AuthErrorResponse ServerError(Exception ex)
	{
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			return Create(
				"Server Error",
				["An internal server error occured"]
			);
		}

		return Create(
			"Server Error",
			[
				ex.GetType().Name,
				ex.Message,
				ex.StackTrace
			]
		);
	}
	
	internal class Examples : IMultipleOpenApiExamplesProvider<AuthErrorResponse>
	{
		public IEnumerable<IOpenApiExample<AuthErrorResponse>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"validationError",
				"Validation Error",
				"Example response with validation errors",
				Create("Validation Error",
				[
					"'Email' is not a valid email address.",
					"The length of 'Password' must be at least 10 characters. You entered 8 characters."
				])
			);

			yield return OpenApiExample.Create(
				"otherError",
				"Other Error",
				"Example response when something went wrong",
				Create("Failed to generate token")
			);
		}
	}
	
	internal class InternalServerErrorExample : ISingleOpenApiExamplesProvider<AuthErrorResponse>
	{
		public IOpenApiExample<AuthErrorResponse> GetExample()
		{
			return OpenApiExample.Create(
				"internalServerError",
				"Internal Server Error",
				"Example response when an internal server error occurs",
				ServerError(new InvalidOperationException("Example Exception"))
			);
		}
	}


}
