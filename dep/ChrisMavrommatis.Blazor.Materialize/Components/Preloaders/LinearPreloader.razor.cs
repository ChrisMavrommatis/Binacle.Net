using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Components;

public partial class LinearPreloader :MaterializeComponentBase
{
	[Parameter]
	public LinearPreloaderType Type { get; set; }

	[Parameter]
	public decimal? Percentage { get; set; }
	
}
