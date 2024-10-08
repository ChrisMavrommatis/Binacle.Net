using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api;

internal interface IApiVersion
{
	int MajorNumber {get; }
	bool Deprecated { get; }
	bool Experimental { get; }

	void ConfigureSwaggerOptions(SwaggerGenOptions options);
}
