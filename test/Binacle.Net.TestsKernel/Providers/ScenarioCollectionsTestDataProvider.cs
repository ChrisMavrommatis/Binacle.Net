using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Providers;

public class ScenarioCollectionsTestDataProvider : CollectionTestDataProvider<List<Scenario>>
{
	private const string scenariosDirectory = "Scenarios";

	public ScenarioCollectionsTestDataProvider(string solutionRootPath) : base(solutionRootPath)
	{
	}

	protected override Dictionary<string, List<Scenario>> InitializeCollections(string solutionRootPath)
	{
		var collections = new Dictionary<string, List<Scenario>>();
		var dirPath = Path.Combine(solutionRootPath, Constants.DataBasePathRoot, scenariosDirectory);
		var scenariosDirectoryInfo = new System.IO.DirectoryInfo(dirPath);
		var fileScenarioReader = new FileScenarioReader();
		
		// Recursive read
		var files = scenariosDirectoryInfo.GetFiles(
			"*.json",
			new EnumerationOptions
			{
				RecurseSubdirectories = true,
				MaxRecursionDepth = 3
			}
		);
		foreach (var file in files)
		{
			var collectionKey = this.GetCollectionKey(file);
			var scenarios = fileScenarioReader.ReadScenarios(file);
			collections.Add(collectionKey, scenarios);
		}
		return collections;
	}

	private string GetCollectionKey(FileInfo file)
	{
		var parts = file.FullName.Split(scenariosDirectory, StringSplitOptions.RemoveEmptyEntries);
		var cleanPath = parts[1].Replace("\\", "/").Trim('/');
		return cleanPath.ToLower().Replace(".json", string.Empty);

	}
}


