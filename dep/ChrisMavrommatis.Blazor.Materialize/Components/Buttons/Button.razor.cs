using ChrisMavrommatis.Blazor.Materialize.Models;
using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Components;

public partial class Button : MaterializeComponentBase
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}
