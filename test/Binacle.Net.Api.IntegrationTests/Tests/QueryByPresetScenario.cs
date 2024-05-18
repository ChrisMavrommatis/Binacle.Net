using Binacle.Net.Api.v1.Requests;
using Binacle.Net.Api.v1.Responses;
using Binacle.Net.TestsKernel.Models;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.Tests;

[Collection(BinacleApiCollection.Name)]
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class QueryByPresetScenario
{
	private readonly BinacleApiFactory sut;
	
	public QueryByPresetScenario(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v1/query/by-preset/{preset}";

	[Theory]
	[ClassData(typeof(Data.Providers.BaselineScenarioTestDataProvider))]
	public Task Baseline(Scenario scenario)
		=> this.RunScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public Task Simple(Scenario scenario)
		=> this.RunScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public Task Complex(Scenario scenario)
		=> this.RunScenarioTest(scenario);

	private async Task RunScenarioTest(Scenario scenario)
	{
		var urlPath = routePath.Replace("{preset}", scenario.BinCollection);

		var request = new PresetQueryRequest
		{
			Items = scenario.Items.Select(x => new Models.Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await this.sut.Client.PostAsJsonAsync(urlPath, request, this.sut.JsonSerializerOptions);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<QueryResponse>();

		result.Should().NotBeNull();
		if (scenario.ExpectedSize != "None")
		{
			result!.Result.Should().Be(v1.Models.ResultType.Success);
			result.Bin.Should().NotBeNull();
			result.Bin!.ID.Should().Be(scenario.ExpectedSize);
		}
		else
		{
			result!.Result.Should().Be(v1.Models.ResultType.Failure);
			result.Bin.Should().BeNull();
		}
	}
}

