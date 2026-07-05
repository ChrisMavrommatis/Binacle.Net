using System.Reflection;

namespace Binacle.TestsKernel.Files;


public static class EmbeddedResourceFileProvider
{
	private static Dictionary<string, List<IFile>> filesByPrefix = new Dictionary<string,List<IFile>>();

	public static List<IFile> ByPrefix(string prefix)
	{
		if (filesByPrefix.TryGetValue(prefix, out var files))
		{
			return files;
		}

		files = new List<IFile>();
		
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
			
			files.Add(file);
		}
		filesByPrefix[prefix] = files;
		return files;
	}
}
