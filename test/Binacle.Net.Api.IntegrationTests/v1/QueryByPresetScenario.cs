using Binacle.Net.TestsKernel.Models;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests;

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
		=> RunScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.SimpleScenarioTestDataProvider))]
	public Task Simple(Scenario scenario)
		=> RunScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.ComplexScenarioTestDataProvider))]
	public Task Complex(Scenario scenario)
		=> RunScenarioTest(scenario);

	private async Task RunScenarioTest(Scenario scenario)
	{
		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(sut.BinCollectionsTestDataProvider);

		var urlPath = routePath.Replace("{preset}", binCollection);

		var request = new Api.v1.Requests.PresetQueryRequest
		{
			Items = scenario.Items.Select(x => new Api.v1.Models.Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await sut.Client.PostAsJsonAsync(urlPath, request, sut.JsonSerializerOptions);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<Api.v1.Responses.QueryResponse>();

		result.Should().NotBeNull();
		if (scenario.Fits)
		{
			result!.Result.Should().Be(Api.v1.Models.ResultType.Success);
			result.Bin.Should().NotBeNull();
			result.Bin!.ID.Should().Be(expectedBin.ID);
		}
		else
		{
			result!.Result.Should().Be(Api.v1.Models.ResultType.Failure);
			result.Bin.Should().BeNull();
		}
	}
}

