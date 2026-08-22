using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Pages;
using Binacle.Net.UIModule.Services;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.UnitTests;

// The page a self-hoster opens to check their own configuration arrived. It reports the off half too, which
// is why FeatureSwitch.All exists alongside FeatureOptions.
[Trait("Behavioral Tests", "Ensures the instance page reports both halves of the switch list")]
public class InstanceModelTests
{
	private static InstanceModel InstanceWith(string environmentName, params (string Feature, string? Path)[] enabled)
	{
		var featureOptions = new FeatureOptions();
		foreach (var (feature, path) in enabled)
		{
			featureOptions.AddFeature(feature, path);
		}

		return new InstanceModel(
			new AppletsService(),
			Options.Create(featureOptions),
			new FakeWebHostEnvironment(environmentName)
		);
	}

	[Fact]
	public void Every_Switch_Reports_Off_On_A_Bare_Instance()
	{
		var page = InstanceWith("Production");

		var on = page.Switches.Where(page.IsOn).ToList();

		page.Switches.ShouldBe(FeatureSwitch.All);
		on.ShouldBeEmpty();
	}

	[Fact]
	public void A_Switch_Reports_On_Only_When_Its_Own_Feature_Was_Registered()
	{
		var page = InstanceWith("Production", ("ScalarUI", "/scalar"));

		var on = page.Switches.Where(page.IsOn).Select(x => x.Feature).ToList();

		on.ShouldBe(["ScalarUI"]);
	}

	// The health path is configurable, so the page cannot build the link itself - it reports where the module
	// that owns the feature said it ended up.
	[Fact]
	public void A_Switch_Reports_The_Path_Whoever_Enabled_It_Recorded()
	{
		var page = InstanceWith("Production", ("HealthChecks", "/_health"));

		var healthCheck = page.Switches.Single(x => x.Feature == "HealthChecks");

		page.PathFor(healthCheck).ShouldBe("/_health");
	}

	[Fact]
	public void A_Switch_That_Answers_On_No_Url_Reports_No_Path()
	{
		var page = InstanceWith("Production", ("DebugEndpoint", null));

		var debugEndpoint = page.Switches.Single(x => x.Feature == "DebugEndpoint");

		page.IsOn(debugEndpoint).ShouldBeTrue();
		page.PathFor(debugEndpoint).ShouldBeNull();
	}

	[Fact]
	public void The_Page_Reports_The_Environment_It_Is_Running_In()
	{
		var page = InstanceWith("Staging");

		var environment = page.Environment;

		environment.ShouldBe("Staging");
	}

	// Neither the service module nor the demo UI appears, on purpose. A row added here without a matching one
	// in FeatureSwitch.All would be a switch the page never mentions.
	[Fact]
	public void The_Switch_List_Names_Each_Feature_Once()
	{
		var page = InstanceWith("Production");

		var features = page.Switches.Select(x => x.Feature).ToList();

		features.Distinct().Count().ShouldBe(features.Count);
		features.ShouldNotContain("UIModule");
	}
}
