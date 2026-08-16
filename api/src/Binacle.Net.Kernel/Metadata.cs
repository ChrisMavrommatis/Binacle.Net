namespace Binacle.Net;

public static class Metadata
{
	public const string Description = "Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.";
	public const string License = "GNU General Public License v3.0";

	public const string GitHub = "https://github.com/binacle-labs/Binacle.Net";
	public const string Dockerhub = "https://hub.docker.com/r/binacle/binacle-net";

	// The version is a deploy fact, not a build fact: the docker image is the only artifact, so BINACLE_VERSION
	// (from ARG VERSION in the Dockerfile, fed by the release tag) is the only version that exists. The
	// assemblies are deliberately not stamped. Do not add a Version property to make the numbers look tidy.
	//
	// Read once at startup, since the environment cannot change under a running process. "Unknown" is correct
	// outside a released image - a plain dotnet run has no tag to report.
	public static string Version { get; } =
		Environment.GetEnvironmentVariable("BINACLE_VERSION") is { Length: > 0 } version
			? version
			: "Unknown";
}
