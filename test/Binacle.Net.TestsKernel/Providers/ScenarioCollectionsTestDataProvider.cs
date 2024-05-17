using Binacle.Net.TestsKernel.Models;
using Newtonsoft.Json;


namespace Binacle.Net.TestsKernel.Providers;

public class ScenarioCollectionsTestDataProvider : CollectionTestDataProvider<List<Scenario>>
{
	public ScenarioCollectionsTestDataProvider(string solutionRootBasePath)
	{
		var dirPath = Path.Combine(solutionRootBasePath, Constants.DataBasePathRoot, "Scenarios/Normal");
		var scenariosDirectoryInfo = new System.IO.DirectoryInfo(dirPath);
		foreach (var scenarioCollectionFileInfo in scenariosDirectoryInfo.GetFiles())
		{
			var scenarioCollectionName = Path.GetFileNameWithoutExtension(scenarioCollectionFileInfo.Name);
			using (var sr = new StreamReader(scenarioCollectionFileInfo.OpenRead()))
			{
				var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(sr.ReadToEnd());
				this.Collections.Add(scenarioCollectionName, scenarios);
			}
		}
	}

	

}


