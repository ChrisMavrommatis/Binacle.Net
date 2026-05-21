using System.Text;

namespace Binacle.Net.v3;

internal static class ResponseDescription
{
	public const string For400BadRequest =  "If the request is invalid.";

	public static string For500InternalServerError = new StringBuilder("If an unexpected error occurs.")
		.AppendLine()
		.AppendLine("Exception details will only be shown when in a development environment.")
		.ToString();

	public const string ForPackResponse200Ok = "Returns an array of results indicating the result per bin.";
	
	public const string ForFitResponse200Ok = "Returns an array of results indicating if a bin can accommodate all the items.";
	
	public const string ForPreset404NotFound = "If the preset does not exist.";
}
