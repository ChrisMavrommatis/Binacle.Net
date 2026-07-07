namespace Binacle.TestReporting;

// Finds the repository root by climbing from the running binary to the folder that holds the solution
// file. Lets a report write to a stable repo-level path no matter how deep the build puts the executable.
public static class RepositoryRoot
{
	public static string Find(string markerFileName = "Binacle.Net.slnx")
	{
		var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
		while (directory is not null)
		{
			if (File.Exists(Path.Combine(directory.FullName, markerFileName)))
			{
				return directory.FullName;
			}

			directory = directory.Parent;
		}

		throw new DirectoryNotFoundException(
			$"Could not find the repository root (no {markerFileName} above the binary).");
	}
}
