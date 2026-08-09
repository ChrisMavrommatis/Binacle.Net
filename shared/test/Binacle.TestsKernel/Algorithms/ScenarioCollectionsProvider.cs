using System.Collections.ObjectModel;
using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Files;

namespace Binacle.TestsKernel.Algorithms;

public static class ScenarioCollectionsProvider
{
	private static Dictionary<string, List<Scenario>> collections;

	public static ReadOnlyDictionary<string, List<Scenario>> Collections => collections.AsReadOnly();
	static ScenarioCollectionsProvider()
	{
		collections = new Dictionary<string, List<Scenario>>();

		var files = EmbeddedResourceFileProvider.ByPrefix("Binacle.TestsKernel.Algorithms.Data.");

		foreach (var file in files)
		{
			var collectionKey = GetCollectionKey(file);
			var scenarios = ScenarioReader.ReadScenarios(file);
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

		if (!collections.TryGetValue(normalizedKey, out var scenarios))
			throw new ArgumentException($"Collection with key {normalizedKey} not found.");

		return scenarios;
	}
}
