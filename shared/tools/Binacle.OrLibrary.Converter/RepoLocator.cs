namespace Binacle.OrLibrary.Converter;

// Finds the one path the tool needs at runtime — the output directory — by walking up from the running binary
// until it sits above it. That keeps the tool clone-independent (no hard-coded absolute path, works wherever the
// repo lives). The raw input doesn't need locating: it travels with the tool as embedded resources.
public static class RepoLocator
{
	public static string FindOutputDir() => FindDirectory("shared", "data", "bischoff-suite");

	private static string FindDirectory(params string[] segments)
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
