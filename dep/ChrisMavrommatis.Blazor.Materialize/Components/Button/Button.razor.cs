using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Components;

public partial class Button : ComponentBase
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}
