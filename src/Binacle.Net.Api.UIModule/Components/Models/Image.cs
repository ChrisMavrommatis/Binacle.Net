using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Models;

internal class Image : IRendableContentComponent
{

	public Image(string src, string alt, int? height = null, int? width = null)
	{
		this.Src = src;
		this.Alt = alt;
		this.Height = height;
		this.Width = width;
		this.Attributes = new Dictionary<string, object>();
	}
	
	public string Src { get; init; }
	public string Alt { get; init; }
	public int? Height { get; init; }
	public int? Width { get; init; }

	public Dictionary<string, object> Attributes { get; set; }
	
	public RenderFragment Render() => builder =>
	{
		builder.OpenElement(0, "img");
		builder.AddAttribute(1, "src", this.Src);
		builder.AddAttribute(2, "alt", this.Alt);
		
		if (this.Height.HasValue)
		{
			builder.AddAttribute(3, "height", this.Height.Value);
		}
		
		if (this.Width.HasValue)
		{
			builder.AddAttribute(4, "width", this.Width.Value);
		}
		
		foreach (var attribute in this.Attributes)
		{
			builder.AddAttribute(5, attribute.Key, attribute.Value);
		}
		
		builder.CloseElement();
	};
}
