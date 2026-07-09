using System.Reflection;

namespace Binacle.ViPaq.TestsKernel.Files;

// Finds packed-data files embedded in this assembly by manifest-name prefix, caching per prefix. The ViPaq
// kernel owns this rather than sharing one: GetExecutingAssembly resolves to this assembly — the one that embeds
// the data — so a shared copy would look in the wrong assembly and find nothing.
public static class EmbeddedResourceFileProvider
{
	private static Dictionary<string, List<IFile>> filesByPrefix = new Dictionary<string, List<IFile>>();

	public static List<IFile> ByPrefix(string prefix)
	{
		if (filesByPrefix.TryGetValue(prefix, out var files))
		{
			return files;
		}

		files = new List<IFile>();

		var assembly = Assembly.GetExecutingAssembly();

		var resources = assembly.GetManifestResourceNames()
			.Where(resource => resource.StartsWith(prefix, StringComparison.Ordinal))
			.OrderBy(resource => resource, StringComparer.Ordinal);

		foreach (var resource in resources)
		{
			// Strip the prefix to get "<family>.<name>.<algorithm>.<extension>".
			var relativePath = resource.Substring(prefix.Length);

			files.Add(new EmbeddedResourceFile(resource, relativePath));
		}

		filesByPrefix[prefix] = files;
		return files;
	}
}
