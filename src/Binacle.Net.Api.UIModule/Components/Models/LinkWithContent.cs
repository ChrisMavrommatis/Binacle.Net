namespace Binacle.Net.Api.UIModule.Components.Models;

internal class LinkWithContent : Link
{
	public LinkWithContent(string title, string url, IRendableContentComponent content, Target target = Target.Self) : base(title, url, target)
	{
		this.Content = content;
	}

	public IRendableContentComponent Content { get; set; }
}
