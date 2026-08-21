namespace Binacle.Net.UIModule.Models;

internal class Applet
{
	public required string Title { get; init; }
	public required string Icon { get; init; }
	public required string ShortDescription { get; init; }
	public required string Description { get; init; }

	// The Razor Page name for asp-page, not a path. A route change needs no edit here.
	public required string Page { get; init; }
}
