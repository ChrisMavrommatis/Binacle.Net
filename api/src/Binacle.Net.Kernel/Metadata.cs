namespace Binacle.Net;

public static class Metadata
{
	public const string Description = "Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.";
	public const string License = "GNU General Public License v3.0";

	public const string GitHub = "https://github.com/ChrisMavrommatis/Binacle.Net";
	public const string Dockerhub = "https://hub.docker.com/r/binacle/binacle-net";

	// The version is a deploy fact, not a build fact. Nothing here is published independently, so the docker image
	// is the only artifact and BINACLE_VERSION - set from the release tag - is the only version that exists. The
	// assemblies are deliberately not stamped; see $.agents/memory/version-only-when-published.md.
	//
	// Read once at startup: the environment cannot change under a running process, and four call sites were each
	// doing this lookup with their own "Unknown" fallback. "Unknown" is correct and expected outside a released
	// image - a plain dotnet run has no tag to report.
	public static string Version { get; } =
		Environment.GetEnvironmentVariable("BINACLE_VERSION") is { Length: > 0 } version
			? version
			: "Unknown";
}
