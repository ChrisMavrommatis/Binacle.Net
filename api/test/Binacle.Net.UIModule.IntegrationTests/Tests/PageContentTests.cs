using System.Net;

namespace Binacle.Net.UIModule.IntegrationTests;

// What each page has to carry for the browser half to work at all. Not the copy - the wiring: the names the
// Alpine components register under, and the bundle paths webpack writes.
//
// The bundles themselves are never requested here. wwwroot/ is generated and gitignored, so on a clone that
// has not run the javascript build there is nothing to serve and every assertion would be red for the wrong
// reason. tooling/smoke asserts the files against a built image; this asserts the page asks for them.
[Trait("Behavioral Tests", "Ensures each demo page carries the wiring its bundle expects")]
[Collection(nameof(UIModuleCollection))]
public class PageContentTests : IClassFixture<UIModuleBinacleApi>
{
	private readonly HttpClient client;

	public PageContentTests(UIModuleBinacleApi api)
	{
		this.client = api.CreateClient();
	}

	private async Task<string> GetPage(string path)
	{
		var response = await this.client.GetAsync(path, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		return await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task The_Index_Links_Every_Applet()
	{
		var page = await this.GetPage("/");

		page.ShouldContain("Packing Demo");
		page.ShouldContain("ViPaq Decoder");
		page.ShouldContain("This Instance");
		page.ShouldContain("href=\"/packing\"");
		page.ShouldContain("href=\"/vipaq\"");
		page.ShouldContain("href=\"/instance\"");
	}

	// The x-data names are registered in packages/binacle-net-ui. Renamed on one side only, the page renders
	// and does nothing at all.
	[Theory]
	[InlineData("/packing", "packing_demo_app")]
	[InlineData("/vipaq", "protocol_decoder_app")]
	public async Task A_Demo_Page_Names_The_Component_That_Drives_It(string path, string componentName)
	{
		var page = await this.GetPage(path);

		page.ShouldContain(componentName);
		page.ShouldContain("packing_visualizer");
		page.ShouldContain("errors_dialog");
	}

	[Theory]
	[InlineData("/packing", "packing_demo")]
	[InlineData("/vipaq", "protocol_decoder")]
	[InlineData("/instance", "instance")]
	public async Task A_Page_Asks_For_Its_Own_Bundle_And_The_Shared_One(string path, string entryName)
	{
		var page = await this.GetPage(path);

		page.ShouldContain($"/_content/Binacle.Net.UIModule/js/{entryName}.js");
		page.ShouldContain("/_content/Binacle.Net.UIModule/js/main.js");
	}

	[Theory]
	[InlineData("/")]
	[InlineData("/packing")]
	[InlineData("/vipaq")]
	[InlineData("/instance")]
	public async Task Every_Page_Asks_For_The_Stylesheet(string path)
	{
		var page = await this.GetPage(path);

		page.ShouldContain("/_content/Binacle.Net.UIModule/css/main.css");
	}

	// An air-gapped install is a normal way to run this, so a page that reaches the internet for an asset is a
	// defect. Anchors a person clicks are fine; a src or href that loads something is not.
	[Theory]
	[InlineData("/")]
	[InlineData("/packing")]
	[InlineData("/vipaq")]
	[InlineData("/instance")]
	public async Task No_Page_Loads_An_Asset_From_The_Internet(string path)
	{
		var page = await this.GetPage(path);

		page.ShouldNotContain("src=\"http");
		page.ShouldNotContain("<link rel=\"stylesheet\" href=\"http");
	}

	// ApiBaseUrl is empty in every shipped configuration, so the demo fetches relative from the API it ships in.
	[Fact]
	public async Task The_Packing_Page_Renders_An_Empty_Api_Base_Url()
	{
		var page = await this.GetPage("/packing");

		page.ShouldContain("packing_demo_app({ baseUrl: '' })");
	}
}
