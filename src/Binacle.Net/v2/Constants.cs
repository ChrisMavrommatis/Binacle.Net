using System.Text;

namespace Binacle.Net.v2;

internal static class ErrorCategory
{
	public const string RequestError = "Malformed request";
	public const string ValidationError = "One or more validation errors occurred";
	public const string ServerError = "An internal server error occurred while processing the request";
}

internal static class ErrorMessage
{
	public const string IsRequired = "Is required.";
	public const string IdMustBeUnique = "Each id must only appear once.";
	public const string MalformedRequestBody = "Malformed request body.";
	public const string GreaterThanZero = "Must be greater than 0.";
}
internal static class ResponseDescription
{
	public const string For400BadRequest = "If the request is invalid.";

	public static string For500InternalServerError = new StringBuilder("If an unexpected error occurs.")
		.AppendLine()
		.AppendLine("Exception details will only be shown when in a development environment.")
		.ToString();

	public const string ForFitResponse200Ok = "Returns an array of results indicating if a bin can accommodate all the items.";
	
	public const string ForPackResponse200Ok = "Returns an array of results indicating the result per bin.";

	public const string ForPreset404NotFound ="If the preset does not exist.";
}
