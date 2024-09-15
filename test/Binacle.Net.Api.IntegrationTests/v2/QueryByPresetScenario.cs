using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.TestsKernel.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.v2;

[Collection(BinacleApiCollection.Name)]
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class QueryByPresetScenario
{
	private readonly BinacleApiFactory sut;

	public QueryByPresetScenario(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v2/query/by-preset/{preset}";

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.BaselineScenarioTestDataProvider))]
	public Task BinaryDecision_Baseline(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.SimpleScenarioTestDataProvider))]
	public Task BinaryDecision_Simple(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(Data.Providers.BinaryDecision.ComplexScenarioTestDataProvider))]
	public Task BinaryDecision_Complex(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	private async Task RunBinaryDecisionScenarioTest(Scenario scenario)
	{
		var presets = sut.Services.GetService<IOptions<BinPresetOptions>>();

		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(sut.BinCollectionsTestDataProvider);
		
		var urlPath = routePath.Replace("{preset}", binCollection);

		var request = new Api.v2.Requests.PresetQueryRequest
		{
			Parameters = new()
			{
				FindSmallestBinOnly = false,
				ReportFittedItems = false,
				ReportUnfittedItems = false,
			},
			Items = scenario.Items.Select(x => new Api.v2.Models.Box
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

		var queryResponse = await response.Content.ReadFromJsonAsync<Api.v2.Responses.QueryResponse>(sut.JsonSerializerOptions);

		queryResponse.Should().NotBeNull();
		queryResponse!.Data.Should()
			.NotBeNull()
			.And.NotBeEmpty()
			.And.HaveCount(presets!.Value.Presets.Count);
		var result = queryResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.Should().NotBeNull();
		result!.Bin.Should().NotBeNull();
		result.Bin.ID.Should().Be(expectedBin.ID);

		if (scenario.Fits)
		{
			// Can't guarantee it as multiple bins are evaluated
			// queryResponse!.Result.Should().Be(Api.v2.Models.ResultType.Success);
			result.Result.Should().Be(Api.v2.Models.BinFitResultStatus.AllItemsFit);
		}
		else
		{
			// Can't guarantee it as multiple bins are evaluated
			// queryResponse!.Result.Should().Be(Api.v2.Models.ResultType.Failure);
			result.Result.Should().NotBe(Api.v2.Models.BinFitResultStatus.AllItemsFit);
		}
	}
}
