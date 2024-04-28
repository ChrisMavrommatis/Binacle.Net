using Binacle.Net.Lib.Tests.Helpers;
using Binacle.Net.Lib.UnitTests.Data.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal abstract class CompactScenarioFileTestDataProvider : ScenarioTestDataProvider
{
	protected CompactScenarioFileTestDataProvider(string filePath)
	{
		var scenarioFileInfo = new FileInfo(filePath);
		using (var sr = new StreamReader(scenarioFileInfo.OpenRead()))
		{
			var compactScenarios = JsonConvert.DeserializeObject<List<CompactScenario>>(sr.ReadToEnd());
			foreach (var compactScenario in compactScenarios)
			{
				var scenario = new Scenario
				{
					Name = compactScenario.Name,
					ExpectedSize = compactScenario.ExpectedSize,
					BinCollection = compactScenario.BinCollection,
					Items = compactScenario.Items.Select(x => DimensionHelper.ParseFromCompactString(x)).ToList()
				};
				_scenarios.Add(scenario.Name, scenario);
			}
		}
	}
}
