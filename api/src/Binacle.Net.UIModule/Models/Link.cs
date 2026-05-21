namespace Binacle.Net.UIModule.Models;

internal class Link 
{
	public Link(string title, string url, Target target = Target.Self)
	{
		this.Title = title;
		this.Url = url;
		this.Target = target;
	}
	
	
	public string Title { get; init; }
	public string Url { get; init; }
	public Target Target { get; } = Target.Self;
	
	public string GetTarget() => this.Target == Target.Blank ? "_blank" : "_self";
}
