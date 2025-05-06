using Binacle.Net.Kernel.OpenApi;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;

namespace Binacle.Net;

public static class OpenApiOptionsExtensions
{
	public static OpenApiOptions AddResponseDescription(this OpenApiOptions options)
	{
		options.AddOperationTransformer<ResponseDescriptionOperationTransformer>();
		return options;
	}
	
	public static OpenApiOptions AddJwtAuthentication(this OpenApiOptions options)
	{
		options.AddDocumentTransformer<JwtBearerSecuritySchemeDocumentTransformer>();
		options.AddOperationTransformer<JwtBearerSecuritySchemeOperationTransformer>();
		return options;
	}
	
	public static OpenApiOptions AddRateLimiterResponse(this OpenApiOptions options)
	{
		options.AddOperationTransformer<RateLimiterResponseOperationTransformer>();
		return options;
	}
	
	public static OpenApiOptions AddEnumStringsSchema(this OpenApiOptions options)
	{
		options.AddSchemaTransformer<EnumStringsSchemaTransformer>();
		return options;
	}
}

