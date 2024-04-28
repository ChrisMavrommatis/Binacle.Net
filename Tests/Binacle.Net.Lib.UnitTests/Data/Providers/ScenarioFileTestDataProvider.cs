using Binacle.Net.Lib.UnitTests.Data.Models;
using Newtonsoft.Json;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal abstract class ScenarioFileTestDataProvider : ScenarioTestDataProvider
{
	public ScenarioFileTestDataProvider(string filePath)
	{
		var scenarioFileInfo = new FileInfo(filePath);
		using (var sr = new StreamReader(scenarioFileInfo.OpenRead()))
		{
			var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(sr.ReadToEnd());
			foreach (var scenario in scenarios)
			{
				_scenarios.Add(scenario.Name, scenario);
			}
		}
	}
}
