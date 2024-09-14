
namespace Binacle.Net.Api.Constants.Errors;

internal static class Categories
{
	public const string RequestError = "Malformed request";
	public const string ValidationError = "One or more validation errors occurred";
	public const string PresetDoesntExist = "The specified preset doesn't exist";
	public const string ServerError = "An internal server error occurred while processing the request";

}

internal static class Messages
{
	public const string IsRequired = "Is required.";
	public const string IdMustBeUnique = "Each id must only appear once.";
	public const string MalformedRequestBody = "Malformed request body.";
	public const string GreaterThanZero = "Must be greater than 0.";

	public static readonly string LessThanUShortMaxValue = string.Format("Must be less than {0}.", ushort.MaxValue);
}

