using System.Collections.ObjectModel;
using Binacle.TestsKernel.Files;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.ScenarioProviders;

public static class ScenarioCollectionsProvider
{
	private static Dictionary<string, List<Scenario>> collections;

	public static ReadOnlyDictionary<string, List<Scenario>> Collections => collections.AsReadOnly();
	static ScenarioCollectionsProvider()
	{
		collections = new Dictionary<string, List<Scenario>>();

		var scenarioReader = new ScenarioReader();
		var files = EmbeddedResourceFileProvider.All();

		foreach (var file in files)
		{
			var collectionKey = GetCollectionKey(file);
			var scenarios = scenarioReader.ReadScenarios(file);
			collections.Add(collectionKey, scenarios);
		}
		
	}

	private static string GetCollectionKey(IFile file)
	{
		return $"{file.Folder.Replace("\\", "/").Trim('/')}/{file.Name}".ToLower();
	}
	
	public static List<Scenario> GetScenarios(string collectionKey)
	{
		var normalizedKey = collectionKey.ToLower();

		if (!collections.ContainsKey(normalizedKey))
			throw new ArgumentException($"Collection with key {normalizedKey} not found.");

		return collections[normalizedKey];
	}
}
