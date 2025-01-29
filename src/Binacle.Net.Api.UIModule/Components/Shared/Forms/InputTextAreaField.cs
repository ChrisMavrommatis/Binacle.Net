using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Binacle.Net.Api.UIModule.Components.Shared.Forms;

public class InputTextAreaField : InputTextArea
{

	[Parameter] 
	public string Id { get; set; } = string.Empty;

	[Parameter] 
	public string Label { get; set; } = string.Empty;

	[Parameter] 
	public string ContainerFieldClass { get; set; } = string.Empty;
	
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		// se border
		builder.OpenElement(0, "div");
		var classes = EditContext?.FieldCssClass(FieldIdentifier);
		builder.AddAttribute(1, "class", $"field label textarea {ContainerFieldClass} {classes}");
		base.BuildRenderTree(builder);
		builder.OpenElement(2, "label");
		builder.AddAttribute(3, "for", Id);
		builder.AddContent(4, Label);
		builder.CloseElement();
		builder.CloseElement();
	}
}
