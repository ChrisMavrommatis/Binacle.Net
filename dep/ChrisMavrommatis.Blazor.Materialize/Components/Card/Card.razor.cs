using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Components;

public partial class Card : ComponentBase
{
	[Parameter]
	public string? Title { get; set; }

	[Parameter]
	public string? Color { get; set; }

	[Parameter]
	public RenderFragment? ImageContent { get; set; }

	[Parameter] 
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public RenderFragment? ActionContent { get; set; }
}
