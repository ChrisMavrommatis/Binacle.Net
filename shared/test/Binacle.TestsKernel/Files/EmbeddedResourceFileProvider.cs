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
			var relativePath = resource.Substring(prefix.Length);

			// No '.' means no extension, so it is not one of the data files. Skip it.
			var lastDotIndex = relativePath.LastIndexOf('.');
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
