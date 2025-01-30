using Binacle.Net.Api.UIModule.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class ErrorsDialog : ComponentBase
{
	private string Title = "One or more Errors occured!";
	private string Button = "Close";
	
	
	[Parameter]
	public Errors? Errors { get; set; }
	
	[Parameter]
	public EventCallback<Errors> ErrorsChanged { get; set; }
	
	private void CloseDialog()
	{
		this.Errors?.Clear();
		this.ErrorsChanged.InvokeAsync(this.Errors);
	}

	private bool HasErrors()
	{
		return this.Errors is not null && this.Errors.Any();
	}
}
