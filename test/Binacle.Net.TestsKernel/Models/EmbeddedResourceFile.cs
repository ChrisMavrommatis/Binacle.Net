using System.Reflection;

namespace Binacle.Net.TestsKernel.Models;

public class EmbeddedResourceFile
{
	private readonly string resourcePath;

	public EmbeddedResourceFile(string resourcePath, string fileName, string folder)
	{
		this.resourcePath = resourcePath;
		Name = fileName;
		Path = $"{folder}/{fileName}";
	}

	public string Name { get; }
	public string Path { get; }

	public Stream OpenRead()
	{
		var assembly = Assembly.GetExecutingAssembly();
		return assembly.GetManifestResourceStream(resourcePath) ?? throw new FileNotFoundException($"Resource {resourcePath} not found");
	}
}
