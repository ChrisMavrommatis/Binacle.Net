using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Models;

internal class MaterialSymbol : IRendableContentComponent
{
	public MaterialSymbol(string name, MaterialSymbolType type = MaterialSymbolType.Outlined)
	{
		this.Name = name;
		this.Type = type;
	}
	
	public string Name { get; init; }
	public MaterialSymbolType Type { get; init; }
	
	public string GetIconClass()
	{
		string @class = "material-symbols";
		switch (this.Type)
		{
			case MaterialSymbolType.Outlined:
				@class += "-outlined";
				break;
			case MaterialSymbolType.Rounded:
				@class += "-rounded";
				break;
			case MaterialSymbolType.Sharp:
				@class += "-sharp";
				break;
		}
		return @class;
	}
	
	public RenderFragment Render() => builder =>
	{
		builder.OpenElement(0, "i");
		builder.AddAttribute(1, "class", this.GetIconClass());
		builder.AddContent(2, this.Name);
		builder.CloseElement();
	};
}

public enum MaterialSymbolType
{
	Outlined,
	Rounded,
	Sharp
}

