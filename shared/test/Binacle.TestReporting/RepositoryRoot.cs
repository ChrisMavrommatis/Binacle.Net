namespace Binacle.TestReporting;

// Finds the repository root by climbing from the running binary to the folder that holds the marker file (the
// solution file by default). Bind once, then Find resolves paths under it. Lets a tool or report write to
// stable repo-level paths no matter how deep the build puts the executable.
//
//   var repo = RepositoryRoot.Bind();
//   var dir = repo.Find("vipaq", "test-vectors", "interop");
public static class RepositoryRoot
{
	public static RepositoryRootLocator Bind(string markerFileName = "Binacle.Net.slnx") => new(markerFileName);
}

// Holds the located root so repeated Find calls just combine segments — the climb happens once, at Bind.
public sealed class RepositoryRootLocator
{
	private readonly string root;

	internal RepositoryRootLocator(string markerFileName)
	{
		var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
		while (directory is not null)
		{
			if (File.Exists(Path.Combine(directory.FullName, markerFileName)))
			{
				this.root = directory.FullName;
				return;
			}

			directory = directory.Parent;
		}

		throw new DirectoryNotFoundException(
			$"Could not find the repository root (no {markerFileName} above the binary).");
	}

	// The repo root, or a path under it: Find() → the root; Find("a", "b") → <root>/a/b.
	public string Find(params string[] segments) => Path.Combine([this.root, .. segments]);
}
