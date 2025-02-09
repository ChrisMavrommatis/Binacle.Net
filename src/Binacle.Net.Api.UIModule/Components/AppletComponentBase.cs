using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components;

public abstract class AppletComponentBase : ComponentBase
{
	protected string? Title => this.applet?.Title;
	protected string? Icon => this.applet?.Icon;
	protected string? ShortDescription => this.applet?.ShortDescription;
	protected string? Description => this.applet?.Description;
	protected abstract string Ref { get; }

	[Inject]
	internal AppletsService? AppletsService { get; set; }

	private Applet? applet;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		this.applet = this.AppletsService?.Applets.FirstOrDefault(x => x.Ref == this.Ref);
	}
}
