using Binacle.Net.TestsKernel.Models;
using System.Reflection;

namespace Binacle.Net.TestsKernel;

public static class EmbeddedResourceFileProvider
{
	// should be Binacle.Net.TestsKernel.Data.
	private static string prefix = $"{typeof(EmbeddedResourceFileProvider).Namespace}.Data.";

	private static Dictionary<string, List<EmbeddedResourceFile>> files = new Dictionary<string,List<EmbeddedResourceFile>>();

	static EmbeddedResourceFileProvider()
	{
		var assembly = Assembly.GetExecutingAssembly();

		var resources = assembly.GetManifestResourceNames()
			.Where(x => x.StartsWith(prefix));

		foreach (var resource in resources)
		{
			// Strip the prefix to get the relative path
			var relativePath = resource.Substring(prefix.Length);
			// Extract folder and file names from the relative path
			var lastDotIndex = relativePath.LastIndexOf('.');

			// Ensure there's at least one '.' to split the file
			if (lastDotIndex < 0)
			{
				continue; 
			}

			// Replace lastDot with _ to avoid confusion with folder names
			relativePath = relativePath.Substring(0, lastDotIndex) + ':' + relativePath.Substring(lastDotIndex + 1);

			var parts = relativePath.Split('.');
			// the last part is the file name
			var fileName = parts.Last();
			// the rest are the folder names
			var folders = parts.Take(parts.Length - 1).ToList();

			var folder = string.Join('/', folders);

			var fileResource = new EmbeddedResourceFile(resource, fileName.Replace(':', '.'), folder);

			if (!files.ContainsKey(folder))
			{
				files.Add(folder, new List<EmbeddedResourceFile>());
			}
			files[folder].Add(fileResource);
		}
	}

	internal static List<EmbeddedResourceFile> GetFiles(string collectionKey)
	{
		if (files.TryGetValue(collectionKey, out var fileNames))
		{
			return fileNames;
		}
		return Enumerable.Empty<EmbeddedResourceFile>().ToList();
	}

	internal static List<EmbeddedResourceFile> GetFilesFromPrefix(string collectionKeyPrefix)
	{
		return files
			.Where(x => x.Key.StartsWith(collectionKeyPrefix))
			.SelectMany(x => x.Value)
			.ToList();

	}
}
