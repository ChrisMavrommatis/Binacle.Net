using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Components;

public class MaterializeButton : MaterializeComponentBase
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}
