using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Pages;
using Binacle.Net.UIModule.Services;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.UnitTests;

// The home page's instance card is the only card carrying a live number. It counts what is on against
// FeatureSwitch.All, so a switch missing from that list makes the denominator wrong rather than the page fail.
[Trait("Behavioral Tests", "Ensures the index cards report the instance without a browser call")]
public class IndexModelTests
{
	private static IndexModel IndexWith(params string[] enabledFeatures)
	{
		var featureOptions = new FeatureOptions();
		foreach (var feature in enabledFeatures)
		{
			featureOptions.AddFeature(feature);
		}

		return new IndexModel(new AppletsService(), Options.Create(featureOptions));
	}

	[Fact]
	public void The_Summary_Counts_Nothing_On_A_Bare_Instance()
	{
		var page = IndexWith();

		var summary = page.InstanceSummary;

		summary.ShouldEndWith($"0 of {FeatureSwitch.All.Count} switched on");
	}

	[Fact]
	public void The_Summary_Counts_Only_The_Switches_The_Page_Lists()
	{
		var page = IndexWith("SwaggerUI", "HealthChecks", "UIModule");

		var summary = page.InstanceSummary;

		summary.ShouldEndWith($"2 of {FeatureSwitch.All.Count} switched on");
	}

	[Fact]
	public void The_Summary_Leads_With_The_Version()
	{
		var page = IndexWith();

		var summary = page.InstanceSummary;

		summary.ShouldStartWith($"{Metadata.Version} - ");
	}

	// Only the instance card has anything the server can add. The other two would have to fetch to say
	// anything, and the home page loads no script.
	[Fact]
	public void Only_The_Instance_Card_Carries_A_Summary()
	{
		var page = IndexWith();

		var summaries = page.Applets.ToDictionary(x => x.Page, page.SummaryFor);

		summaries["/Instance"].ShouldNotBeNullOrWhiteSpace();
		summaries["/Packing"].ShouldBeNull();
		summaries["/Vipaq"].ShouldBeNull();
	}
}
