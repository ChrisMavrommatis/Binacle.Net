using System.Net;
using System.Net.Http.Json;

namespace Binacle.Net.UIModule.IntegrationTests;

// Who gets a web page and who does not. With the demo on, the whole host has an error page, and the only thing
// keeping an API 404 from arriving as HTML is ReservedPathOptions - so this is the test that fails when a
// module maps a path and forgets to declare it.
[Trait("Behavioral Tests", "Ensures only page routes answer with a web page")]
[Collection(nameof(UIModuleCollection))]
public class ErrorPageRoutingTests : IClassFixture<UIModuleBinacleApi>
{
	private readonly HttpClient client;

	public ErrorPageRoutingTests(UIModuleBinacleApi api)
	{
		this.client = api.CreateClient();
	}

	[Theory]
	[InlineData("/")]
	[InlineData("/packing")]
	[InlineData("/vipaq")]
	[InlineData("/instance")]
	[InlineData("/error/404")]
	public async Task A_Page_Route_Answers_With_Html(string path)
	{
		var response = await this.client.GetAsync(path, TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		response.Content.Headers.ContentType?.MediaType.ShouldBe("text/html");
	}

	[Theory]
	[InlineData("/nope")]
	[InlineData("/packing/nope")]
	public async Task An_Unknown_Page_Route_Answers_With_The_Error_Page(string path)
	{
		var response = await this.client.GetAsync(path, TestContext.Current.CancellationToken);
		var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
		response.Content.Headers.ContentType?.MediaType.ShouldBe("text/html");
		body.ShouldContain("That page does not exist.");
	}

	// The four prefixes Program.cs reserves, the one the UI module reserves for its own bundle, and the two the
	// Diagnostics module reserves. None of them may come back as a page.
	[Theory]
	[InlineData("/api/nope")]
	[InlineData("/api/v3/nope")]
	[InlineData("/api/v4/nope")]
	[InlineData("/openapi/nope.json")]
	[InlineData("/swagger/nope")]
	[InlineData("/scalar/nope")]
	[InlineData("/_content/nope.js")]
	public async Task A_Reserved_Path_Answers_Without_A_Web_Page(string path)
	{
		var response = await this.client.GetAsync(path, TestContext.Current.CancellationToken);
		var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
		response.Content.Headers.ContentType?.MediaType.ShouldNotBe("text/html");
		body.ShouldNotContain("<html");
	}

	// The everyday case, and the one a client sees: a bad request body is a validation problem, not a page.
	[Fact]
	public async Task A_Rejected_Api_Request_Answers_With_Problem_Details_Rather_Than_A_Page()
	{
		var response = await this.client.PostAsJsonAsync("/api/v3/pack/by-custom", new { }, TestContext.Current.CancellationToken);
		var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

		response.IsSuccessStatusCode.ShouldBeFalse();
		response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
		body.ShouldNotContain("<html");
	}

	// The presets list is the one API route the demo's own pages depend on, so a page-shaped answer here would
	// break the instance page rather than the caller.
	[Fact]
	public async Task A_Served_Api_Route_Is_Untouched_By_The_Error_Page()
	{
		var response = await this.client.GetAsync("/api/v4/presets", TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
	}
}
