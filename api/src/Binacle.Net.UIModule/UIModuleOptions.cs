namespace Binacle.Net.UIModule;

internal class UIModuleOptions
{
	// Empty means the demo fetches relative, from the API it ships in. That is the only shipped value.
	public string ApiBaseUrl { get; set; } = string.Empty;
}
