﻿using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Binacle.Net.Api.IntegrationTests.v1;

[Collection(BinacleApiCollection.Name)]
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class QueryByCustomScenario
{
	private readonly BinacleApiFactory sut;

	public QueryByCustomScenario(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v1/query/by-custom";

	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public Task BinaryDecision_Baseline(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public Task BinaryDecision_Simple(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public Task BinaryDecision_Complex(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	private async Task RunBinaryDecisionScenarioTest(Scenario scenario)
	{
		var presets = sut.Services.GetService<IOptions<BinPresetOptions>>();
		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(this.sut.BinCollectionsTestDataProvider);

		presets!.Value.Presets.ShouldContainKey(binCollection);

		var binPresetOption = presets.Value.Presets[binCollection];

		var request = new Api.v1.Requests.CustomQueryRequest
		{
			Bins = binPresetOption.Bins.Select(x => new Api.v1.Models.Bin
			{
				ID = x.ID,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList(),
			Items = scenario.Items.Select(x => new Api.v1.Models.Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<Api.v1.Responses.QueryResponse>();

		result.ShouldNotBeNull();

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();
		if (scenarioResult.Fits)
		{
			result!.Result.ShouldBe(Api.v1.Models.ResultType.Success);
			result.Bin.ShouldNotBeNull();
			result.Bin!.ID.ShouldBe(expectedBin.ID);
		}
		else
		{
			result!.Result.ShouldBe(Api.v1.Models.ResultType.Failure);
			result.Bin.ShouldBeNull();
		}
	}
}

