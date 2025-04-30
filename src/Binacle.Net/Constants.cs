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
	public static string For400BadRequest =  "If the request is invalid.";

	public static string For500InternalServerError = new StringBuilder("If an unexpected error occurs.")
		.AppendLine()
		.AppendLine("Exception details will only be shown when in a development environment.")
		.ToString();

	public static string ForPresets200OK = "Returns all of the configured presets wth the associated bins.";

	public static string ForPresets404NotFound = "If no presets are configured.";

	public static string ForQueryResponse200OK = "Returns the bin that fits all of the items, or empty if they don't fit.";
	
	public static string ForFitResponse200OK = "Returns an array of results indicating if a bin can accommodate all the items.";
	
	public static string ForPackResponse200OK = "Returns an array of results indicating the result per bin.";

	public static string ForPreset404NotFound ="If the preset does not exist.";

	
}
