namespace Binacle.Net.v4;

internal static class ResponseDescription
{
	public const string For400BadRequest = "The request is invalid.";

	public const string For500InternalServerError =
		"An unexpected error occurred. Exception details are shown only in a development environment.";

	public const string ForPreset404NotFound = "The preset does not exist.";
}
