using Microsoft.AspNetCore.OpenApi;

namespace Binacle.Net.Kernel.OpenApi;

public interface IOpenApiDocument
{
	string Name { get; }
	string Title { get; }
	string Version { get; }
	bool IsDeprecated { get; }
	bool IsExperimental { get; }

	void Configure(OpenApiOptions options);
}
