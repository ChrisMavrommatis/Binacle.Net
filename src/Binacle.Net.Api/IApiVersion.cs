using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Binacle.Net.Api;

internal interface IApiVersion
{
	int MajorNumber {get; }
	bool Deprecated { get; }
	bool Experimental { get; }

	void ConfigureSwaggerOptions(SwaggerGenOptions options, IApiVersionDescriptionProvider provider);
	void ConfigureSwaggerUI(SwaggerUIOptions options, IApiVersionDescriptionProvider provider);
}
