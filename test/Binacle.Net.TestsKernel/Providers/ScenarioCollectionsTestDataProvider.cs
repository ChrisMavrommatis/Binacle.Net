using Binacle.Net.TestsKernel.Data;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Providers;

public class ScenarioCollectionsTestDataProvider : CollectionTestDataProvider<List<Scenario>>
{
	private const string scenariosPrefixKey = "Scenarios";

	public ScenarioCollectionsTestDataProvider()
	{
	}

	protected override Dictionary<string, List<Scenario>> InitializeCollections()
	{
		var collections = new Dictionary<string, List<Scenario>>();
		var fileScenarioReader = new EmbeddedResourceFileScenarioReader();
		var files = EmbeddedResourceFileProvider.GetFilesFromPrefix(scenariosPrefixKey);

		foreach (var file in files)
		{
			var collectionKey = this.GetCollectionKey(file);
			var scenarios = fileScenarioReader.ReadScenarios(file);
			collections.Add(collectionKey, scenarios);
		}
		return collections;
	}

	private string GetCollectionKey(EmbeddedResourceFile file)
	{
		var path = file.Path.Replace(scenariosPrefixKey, string.Empty);
		var cleanPath = path.Replace("\\", "/").Trim('/');
		return cleanPath.ToLower().Replace(".json", string.Empty);

	}
}


