using System.Collections.ObjectModel;
using Binacle.TestsKernel.Files;

namespace Binacle.TestsKernel.ResultSelection;

public static class ScenarioCollectionsProvider
{
    private static Dictionary<string, List<Models.Scenario>> collections;

    public static ReadOnlyDictionary<string, List<Models.Scenario>> Collections => collections.AsReadOnly();
    static ScenarioCollectionsProvider()
    {
        collections = new Dictionary<string, List<Models.Scenario>>();

        var scenarioReader = new ScenarioReader();
        var files = EmbeddedResourceFileProvider.ByPrefix("Binacle.TestsKernel.ResultSelection.Data.");

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
	
    public static List<Models.Scenario> GetScenarios(string collectionKey)
    {
        var normalizedKey = collectionKey.ToLower();

        if (!collections.ContainsKey(normalizedKey))
            throw new ArgumentException($"Collection with key {normalizedKey} not found.");

        return collections[normalizedKey];
    }
}
