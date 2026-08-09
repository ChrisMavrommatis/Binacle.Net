namespace Binacle.Net;

public static class Metadata
{
	public const string Description = "Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.";
	public const string License = "GNU General Public License v3.0";

	public const string GitHub = "https://github.com/ChrisMavrommatis/Binacle.Net";
	public const string Dockerhub = "https://hub.docker.com/r/binacle/binacle-net";

	// The version is a deploy fact, not a build fact. Nothing here is published independently, so the docker image
	// is the only artifact and BINACLE_VERSION - set from ARG VERSION in the Dockerfile, fed by the release tag -
	// is the only version that exists. The assemblies are deliberately not stamped: a per-component number could
	// only repeat the image's or lie about it, and "which build is this" is already answered by the image tag.
	// That changes only if something here starts shipping on its own - a NuGet PackageId, or a TS package losing
	// `private`. Until then, do not add a Version property to make the numbers look tidy.
	//
	// Read once at startup: the environment cannot change under a running process, and four call sites were each
	// doing this lookup with their own "Unknown" fallback. "Unknown" is correct and expected outside a released
	// image - a plain dotnet run has no tag to report.
	public static string Version { get; } =
		Environment.GetEnvironmentVariable("BINACLE_VERSION") is { Length: > 0 } version
			? version
			: "Unknown";
}
