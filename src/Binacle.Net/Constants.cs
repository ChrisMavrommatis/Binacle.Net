using System.Text;

namespace Binacle.Net.Constants;

internal static class ErrorCategory
{
	public const string RequestError = "Malformed request";
	public const string ValidationError = "One or more validation errors occurred";
	public const string PresetDoesntExist = "The specified preset doesn't exist";
	public const string ServerError = "An internal server error occurred while processing the request";
}

internal static class ErrorMessage
{
	public const string IsRequired = "Is required.";
	public const string IdMustBeUnique = "Each id must only appear once.";
	public const string MalformedRequestBody = "Malformed request body.";
	public const string GreaterThanZero = "Must be greater than 0.";
	public const string VolumeOverflow = "The dimensions provided result in a volume overflow.";

	public static readonly string LessThanUShortMaxValue = string.Format("Must be less than {0}.", ushort.MaxValue);
}

internal static class ResponseDescription
{
	public static string For400BadRequest = new StringBuilder("**Bad Request**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("If the request is invalid.")
		.AppendLine()
		.ToString();

	public static string For500InternalServerError = new StringBuilder("**Internal Server Error**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("If an unexpected error occurs.")
		.AppendLine()
		.AppendLine("Exception details will only be shown when in a development environment.")
		.AppendLine()
		.ToString();

	public static string ForPresets200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("Returns all of the configured presets wth the associated bins.")
		.AppendLine()
		.ToString();

	public static string ForPresets404NotFound = new StringBuilder("**Not Found**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("If no presets are configured.")
		.AppendLine()
		.ToString();

	public static string ForQueryResponse200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("Returns the bin that fits all of the items, or empty if they don't fit.")
		.AppendLine()
		.ToString();
	
	public static string ForFitResponse200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("Returns an array of results indicating if a bin can accommodate all the items.")
		.AppendLine()
		.ToString();
	
	public static string ForPackResponse200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("Returns an array of results indicating the result per bin.")
		.AppendLine()
		.ToString();

	public static string ForPreset404NotFound = new StringBuilder("**Not Found**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("If the preset does not exist.")
		.AppendLine()
		.ToString();

	
}
