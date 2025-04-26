namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class ErrorResponse
{
	public required string Message { get; set; }
	public string[]? Errors { get; set; }
	

	internal static ErrorResponse Create(string message, string[]? errors = null)
	{
		return new ErrorResponse
		{
			Message = message,
			Errors = errors
		};
	}

	internal static ErrorResponse MalformedRequest =>
		ErrorResponse.Create(
			"Malformed request",
			["Malformed request body"]
		);

	internal static ErrorResponse ValidationError(string[] errors)
	{
		return ErrorResponse.Create(
			"Validation Error",
			errors
		);
	}
	
	internal static ErrorResponse ServerError(Exception exception)
	{
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			return ErrorResponse.Create(
				"Server Error",
				["An internal server error occured"]
			);
		}

		return ErrorResponse.Create(
			"Server Error",
			[
				exception.GetType().Name,
				exception.Message,
				exception.StackTrace ?? string.Empty
			]
		);
	}

	internal static ErrorResponse IdToGuidParameterError =>
		ErrorResponse.Create(
			"Parameter Error",
			["The provided value is not a valid Guid"]
		);
	
	internal static ErrorResponse PageNumberError =>
		ErrorResponse.Create(
			"Parameter Error",
			["The provided page number is not valid"]
		);
	
	internal static ErrorResponse PageSizeError =>
		ErrorResponse.Create(
			"Parameter Error",
			["The provided page size is not valid"]
		);
}
