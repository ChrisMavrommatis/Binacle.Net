namespace Binacle.Net.Api.UIModule.Models;

public class Badge
{
	public Badge(string name, string image)
	{
		this.Name = name;
		this.Image = image;
	}
	public Badge(string name, string url, string image)
	{
		this.Name = name;
		this.Url = url;
		this.Image = image;
	}
	public string Name { get;  }
	public string? Url { get;  }
	public string Image { get; }
}
