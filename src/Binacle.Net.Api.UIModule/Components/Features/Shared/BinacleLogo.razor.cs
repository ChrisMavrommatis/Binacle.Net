using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Features.Shared;

public partial class BinacleLogo : ComponentBase
{
	[Inject]
	private NavigationManager? NavigationManager { get; set; }

	private bool IsHomepage { get; set; }

	protected override void OnInitialized()
	{
		var uri = new Uri(this.NavigationManager!.Uri);

		this.IsHomepage = uri.LocalPath == "/";
	}
}
