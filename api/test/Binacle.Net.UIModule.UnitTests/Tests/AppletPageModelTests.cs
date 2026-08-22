using Binacle.Net.UIModule.Pages;
using Binacle.Net.UIModule.Services;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.UnitTests;

// Each demo page names the applet it is, and AppletPageModel takes the first match. A page name that no
// longer appears in the list throws on construction, which is a 500 on that route and nothing before it.
[Trait("Behavioral Tests", "Ensures every demo page resolves its own applet")]
public class AppletPageModelTests
{
	private static readonly AppletsService appletsService = new();

	[Fact]
	public void The_Packing_Page_Resolves_Its_Applet()
	{
		var options = Options.Create(new UIModuleOptions { ApiBaseUrl = string.Empty });

		var page = new PackingModel(appletsService, options);

		page.Applet.Page.ShouldBe("/Packing");
	}

	[Fact]
	public void The_Vipaq_Page_Resolves_Its_Applet()
	{
		var page = new VipaqModel(appletsService);

		page.Applet.Page.ShouldBe("/Vipaq");
	}

	[Fact]
	public void The_Instance_Page_Resolves_Its_Applet()
	{
		var featureOptions = Options.Create(new FeatureOptions());

		var page = new InstanceModel(appletsService, featureOptions, new FakeWebHostEnvironment("Production"));

		page.Applet.Page.ShouldBe("/Instance");
	}

	// The demo fetches relative when this is empty, so a trailing slash would produce "//api/v3/...".
	[Theory]
	[InlineData("", "")]
	[InlineData("https://api.binacle.net", "https://api.binacle.net")]
	[InlineData("https://api.binacle.net/", "https://api.binacle.net")]
	public void The_Packing_Page_Renders_The_Api_Base_Url_Without_A_Trailing_Slash(string configured, string expected)
	{
		var options = Options.Create(new UIModuleOptions { ApiBaseUrl = configured });

		var page = new PackingModel(appletsService, options);

		page.ApiBaseUrl.ShouldBe(expected);
	}
}
