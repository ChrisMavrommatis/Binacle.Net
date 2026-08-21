using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Binacle.Net.UIModule.Pages;

// The title and copy of a demo page come from AppletsService, so the index cards and the page itself cannot
// disagree.
internal abstract class AppletPageModel : PageModel
{
	protected AppletPageModel(AppletsService appletsService, string appletPage)
	{
		this.Applet = appletsService.Applets.First(x => x.Page == appletPage);
	}

	public Applet Applet { get; }
}
