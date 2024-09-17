using ChrisMavrommatis.Blazor.Materialize.Models;
using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Components;

public partial class CircularPreloader :MaterializeComponentBase
{
	[Parameter]
	public string SpinnerColor { get; set; }
}
