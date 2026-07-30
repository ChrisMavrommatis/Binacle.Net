using System.Reflection;

namespace Binacle.ViPaq.TestsKernel.Files;

internal class EmbeddedResourceFile : IFile
{
	private readonly string resourcePath;

	public EmbeddedResourceFile(string resourcePath, string relativePath)
	{
		this.resourcePath = resourcePath;

		// relativePath is "<family>.<name>.<algorithm>.<extension>", e.g. "bischoff-suite.orlib_thpack1.ffd.json".
		// Family folders and names carry no dots, so a plain split gives exactly four parts.
		var parts = relativePath.Split('.');
		if (parts.Length != 4)
		{
			throw new ArgumentException(
				$"Packed-data resource '{resourcePath}' is not the expected <family>.<name>.<algorithm>.<extension> shape.");
		}

		this.Family = parts[0];
		this.Name = parts[1];
		this.Algorithm = parts[2];
		this.Extension = parts[3];
	}

	public string Family { get; }
	public string Name { get; }
	public string Algorithm { get; }
	public string Extension { get; }

	public Stream OpenRead()
	{
		var assembly = Assembly.GetExecutingAssembly();
		return assembly.GetManifestResourceStream(resourcePath)
			?? throw new FileNotFoundException($"Resource {resourcePath} not found");
	}
}
