using Binacle.Net.UIModule.Components;

namespace Binacle.Net.UIModule.Models;

internal class LinkWithContent : Link
{
	public LinkWithContent(
		string title, 
		string url, 
		IRendableContentComponent content, 
		Target target = Target.Self
		) : base(title, url, target)
	{
		this.Content = content;
	}

	public IRendableContentComponent Content { get; set; }
}
