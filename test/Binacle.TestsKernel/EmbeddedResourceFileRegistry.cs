using System.Reflection;

namespace Binacle.TestsKernel;


public static class EmbeddedResourceFileRegistry
{
	// should be Binacle.TestsKernel.Data.
	private static string prefix = $"Binacle.TestsKernel.Data.";

	private static Dictionary<string, List<IFile>> files = new Dictionary<string,List<IFile>>();

	static EmbeddedResourceFileRegistry()
	{
		var assembly = Assembly.GetExecutingAssembly();

		var resources = assembly.GetManifestResourceNames()
			.Where(x => x.StartsWith(prefix));

		foreach (var resource in resources)
		{
			// Strip the prefix to get the relative path
			var relativePath = resource.Substring(prefix.Length);
			
			// Find the last '.' to separate file name and extension
			var lastDotIndex = relativePath.LastIndexOf('.');

			// Ensure there's at least one '.' to split the file
			if (lastDotIndex < 0)
			{
				continue; 
			}

			var file = new EmbeddedResourceFile(resource, relativePath);
			
			if (!files.ContainsKey(file.Folder))
			{
				files.Add(file.Folder, new List<IFile>());
			}
			files[file.Folder].Add(file);
		}
	}

	public static List<IFile> All()
	{
		return files.SelectMany(x => x.Value).ToList();
	}
}
