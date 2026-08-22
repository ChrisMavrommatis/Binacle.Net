using System.Collections.Generic;
using System.Linq;
using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.Pages;

internal class IndexModel : PageModel
{
	private readonly AppletsService appletsService;
	private readonly IOptions<FeatureOptions> featureOptions;

	public IndexModel(AppletsService appletsService, IOptions<FeatureOptions> featureOptions)
	{
		this.appletsService = appletsService;
		this.featureOptions = featureOptions;
	}

	public IReadOnlyList<Applet> Applets => this.appletsService.Applets;

	// The instance card carries what the server already knows. The preset count is not here on purpose: it
	// comes from the API in the browser, and the home page loads no script.
	public string InstanceSummary
	{
		get
		{
			var on = FeatureSwitch.All.Count(x => this.featureOptions.Value.IsFeatureEnabled(x.Feature));
			return $"{Metadata.Version} - {on} of {FeatureSwitch.All.Count} switched on";
		}
	}

	public string? SummaryFor(Applet applet)
		=> applet.Page == "/Instance" ? this.InstanceSummary : null;

	public void OnGet()
	{
	}
}
