using Binacle.Net.TestsKernel.Helpers;
using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;


namespace Binacle.Net.TestsKernel.Providers;

public class CompactScenarioCollectionsTestDataProvider : CollectionTestDataProvider<List<Scenario>>
{
	public CompactScenarioCollectionsTestDataProvider(string solutionRootBasePath)
	{
		var dirPath = Path.Combine(solutionRootBasePath, Constants.DataBasePathRoot, "Scenarios/Compact");
		var scenariosDirectoryInfo = new System.IO.DirectoryInfo(dirPath);
		foreach (var scenarioCollectionFileInfo in scenariosDirectoryInfo.GetFiles())
		{
			var scenarioCollectionName = Path.GetFileNameWithoutExtension(scenarioCollectionFileInfo.Name);
			using (var sr = new StreamReader(scenarioCollectionFileInfo.OpenRead()))
			{
				var scenarios = new List<Scenario>();
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
					scenarios.Add(scenario);
				}
				this.Collections.Add(scenarioCollectionName, scenarios);
			}
		}
	}
}


