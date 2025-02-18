using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Binacle.Net.Api.UIModule.Components.Shared.Forms;

public class SelectField<TValue> : InputSelect<TValue>
	where TValue : notnull
{
	[Parameter] 
	public string Id { get; set; } = string.Empty;
	
	[Parameter] 
	public string Label { get; set; } = string.Empty;

	[Parameter]
	public string ContainerFieldClass { get; set; } = string.Empty;
	
	[Parameter]
	public Dictionary<TValue, string> Options { get; set; } = new();

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "div");
		var classes = EditContext?.FieldCssClass(FieldIdentifier);
		builder.AddAttribute(1, "class", $"field {classes} {ContainerFieldClass}");
		this.ChildContent = (builder2) =>
		{
			foreach (var (key, value) in this.Options)
			{
				builder2.OpenElement(0, "option");
				builder2.AddAttribute(1, "value", key);
				if (this.Value is not null && this.Value.Equals(key))
				{
					builder2.AddAttribute(2, "selected", true);
				}
				builder2.AddContent(3, value);
				builder2.CloseElement();
			}
		};
		base.BuildRenderTree(builder);
		builder.OpenElement(2, "i");
		builder.AddContent(3, "arrow_drop_down");
		builder.CloseElement();
		builder.CloseElement();
	}
}
