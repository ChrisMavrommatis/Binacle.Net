using System.Collections.ObjectModel;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Providers;

public class ScenarioCollectionsDataProvider
{
	private readonly Dictionary<string, List<Scenario>> collections;

	public ReadOnlyDictionary<string, List<Scenario>> Collections => this.collections.AsReadOnly();
	public ScenarioCollectionsDataProvider()
	{
		this.collections = new Dictionary<string, List<Scenario>>();

		var scenarioReader = new ScenarioReader();
		var files = EmbeddedResourceFileRegistry.All();

		foreach (var file in files)
		{
			var collectionKey = this.GetCollectionKey(file);
			var scenarios = scenarioReader.ReadScenarios(file);
			this.collections.Add(collectionKey, scenarios);
		}
		
	}

	private string GetCollectionKey(IFile file)
	{
		return $"{file.Folder.Replace("\\", "/").Trim('/')}/{file.Name}".ToLower();
	}
	
	public List<Scenario> GetScenarios(string collectionKey)
	{
		var normalizedKey = collectionKey.ToLower();

		if (!this.collections.ContainsKey(normalizedKey))
			throw new ArgumentException($"Collection with key {normalizedKey} not found.");

		return this.collections[normalizedKey];
	}
}
