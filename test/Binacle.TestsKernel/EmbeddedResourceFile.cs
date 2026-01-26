using System.Reflection;

namespace Binacle.TestsKernel;

internal class EmbeddedResourceFile : IFile
{
	private readonly string resourcePath;
	private readonly string relativePath;

	public EmbeddedResourceFile(string resourcePath, string relativePath)
	{
		this.resourcePath = resourcePath;
		this.relativePath = relativePath;
		var lastDotIndex = relativePath.LastIndexOf('.');


		var relativePathWithoutExtension = relativePath.Substring(0, lastDotIndex);
		this.Extension = relativePath.Substring(lastDotIndex + 1);
			
		var relativePartPaths = relativePathWithoutExtension.Split('.');
		this.Name = relativePartPaths.Last();
		
		var folderPaths = relativePartPaths.Take(relativePartPaths.Length - 1).ToList();
		this.Folder = string.Join('/', folderPaths);
	}

	public string Name { get; }
	public string Extension { get; }
	public string Path => $"{Folder}/{Name}.{Extension}";
	public string Folder { get; }

	public Stream OpenRead()
	{
		var assembly = Assembly.GetExecutingAssembly();
		return assembly.GetManifestResourceStream(resourcePath) ?? throw new FileNotFoundException($"Resource {resourcePath} not found");
	}
}
