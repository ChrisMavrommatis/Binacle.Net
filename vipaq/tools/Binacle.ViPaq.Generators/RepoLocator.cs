namespace Binacle.ViPaq.Generators;

// Finds vipaq/test-vectors/interop by walking up from the running assembly. Keeps the tool
// clone-independent — no hard-coded absolute path, works wherever the repo sits on disk.
public static class RepoLocator
{
	public static string FindInteropDir()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null)
		{
			var candidate = Path.Combine(directory.FullName, "vipaq", "test-vectors", "interop");
			if (Directory.Exists(candidate))
			{
				return candidate;
			}

			directory = directory.Parent;
		}

		throw new DirectoryNotFoundException(
			$"Could not find vipaq/test-vectors/interop by walking up from {AppContext.BaseDirectory}");
	}
}
