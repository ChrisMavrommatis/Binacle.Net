using System.Net;

namespace Binacle.Net.UIModule.IntegrationTests;

// The image ships the demo behind a switch, and most deployments turn it off. Nothing it added may survive.
[Trait("Behavioral Tests", "Ensures the demo leaves nothing behind when it is switched off")]
[Collection(nameof(UIModuleCollection))]
public class UIModuleOffTests : IClassFixture<UIModuleOffBinacleApi>
{
	private readonly HttpClient client;

	public UIModuleOffTests(UIModuleOffBinacleApi api)
	{
		this.client = api.CreateClient();
	}

	[Theory]
	[InlineData("/")]
	[InlineData("/packing")]
	[InlineData("/vipaq")]
	[InlineData("/instance")]
	[InlineData("/error/404")]
	public async Task A_Page_Route_Is_Gone(string path)
	{
		var response = await this.client.GetAsync(path, TestContext.Current.CancellationToken);
		var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
		body.ShouldNotContain("<html");
	}

	[Fact]
	public async Task The_Api_Still_Serves()
	{
		var response = await this.client.GetAsync("/api/v4/presets", TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
	}
}
