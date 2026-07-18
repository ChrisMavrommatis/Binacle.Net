namespace Binacle.Net.v3;

internal static class ResponseDescription
{
	public const string For400BadRequest = "The request is invalid.";

	public const string For500InternalServerError =
		"An unexpected error occurred. Exception details are shown only in a development environment.";

	public const string ForPackResponse200Ok = "Returns an array of results indicating the result per bin.";

	public const string ForFitResponse200Ok = "Returns an array of results indicating if a bin can accommodate all the items.";

	public const string ForPreset404NotFound = "The preset does not exist.";
}
