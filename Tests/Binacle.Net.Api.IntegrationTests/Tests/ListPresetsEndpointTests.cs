using Binacle.Net.Api.Configuration.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.Tests;

public class ListPresetsEndpointTests : IClassFixture<WebApplicationFactory<Binacle.Net.Api.Program>>
{
	private const string routePath = "/api/v1/presets";
	private readonly WebApplicationFactory<Binacle.Net.Api.Program> sut;
	private readonly HttpClient client;

	public ListPresetsEndpointTests(WebApplicationFactory<Binacle.Net.Api.Program> sut)
	{
		this.sut = sut
			.WithWebHostBuilder(builder =>
			{
				builder.UseEnvironment("Development");
				builder.ConfigureTestServices(services =>
				{
					services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
				});
			});

		this.client = this.sut.CreateClient();
	}

	public void Dispose()
	{
		this.client.Dispose();
		this.sut.Dispose();
	}

	[Fact]
	public async Task Get_WithPresetsConfigured_Returns200Ok()
	{
		var response = await this.client.GetAsync(routePath);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
	}

	[Fact]
	public async Task Get_WithoutPresetsConfigured_Returns404NotFound()
	{
		// remove presets for this test
		var presetOptions = this.sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
		presetOptions.Value.Presets.Clear();

		var response = await this.client.GetAsync(routePath);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
	}

	
}
