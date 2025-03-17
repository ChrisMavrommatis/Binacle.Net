using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Binacle.Net.Api.UIModule.Components.Shared;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

public class MaterialSymbol : BinacleComponentBase, IRendableContentComponent
{
	[Parameter]
	public string Name { get; set; }
	
	[Parameter]
	public MaterialSymbolType Type { get; set; } = MaterialSymbolType.Outlined;
	
	private string GetIconClass()
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

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "i");
		builder.AddAttribute(1, "class", $"{this.GetIconClass()} {this._classes}");
		builder.AddContent(2, this.Name);
		builder.CloseElement();
	}

	public RenderFragment Render() => builder => BuildRenderTree(builder);
}

public enum MaterialSymbolType
{
	Outlined,
	Rounded,
	Sharp
}

