using Binacle.Net.Kernel.OpenApi;
using Microsoft.AspNetCore.OpenApi;

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
}
