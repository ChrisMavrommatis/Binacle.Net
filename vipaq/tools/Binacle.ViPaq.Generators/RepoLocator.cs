namespace Binacle.ViPaq.Generators;

// Finds shared-vector directories by walking up from the running assembly. Keeps the tool
// clone-independent — no hard-coded absolute path, works wherever the repo sits on disk.
public static class RepoLocator
{
	public static string FindTestVectorsDir() => FindDir("vipaq", "test-vectors");

	public static string FindInteropDir() => FindDir("vipaq", "test-vectors", "interop");

	private static string FindDir(params string[] segments)
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null)
		{
			var candidate = Path.Combine([directory.FullName, .. segments]);
			if (Directory.Exists(candidate))
			{
				return candidate;
			}

			directory = directory.Parent;
		}

		throw new DirectoryNotFoundException(
			$"Could not find {Path.Combine(segments)} by walking up from {AppContext.BaseDirectory}");
	}
}
