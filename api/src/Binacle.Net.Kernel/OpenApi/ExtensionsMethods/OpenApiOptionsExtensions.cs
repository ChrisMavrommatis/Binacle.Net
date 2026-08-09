using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.AspNetCore.OpenApi;

namespace Binacle.Net;

public static class OpenApiOptionsExtensions
{
	public static OpenApiOptions AddResponseDescription(this OpenApiOptions options)
	{
		options.AddOperationTransformer<ResponseDescriptionOperationTransformer>();
		return options;
	}

	public static OpenApiOptions AddOperationIds(this OpenApiOptions options)
	{
		options.AddOperationTransformer<OperationIdOperationTransformer>();
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
		options.AddDocumentTransformer<RequiredNullableSchemaDocumentTransformer>();
		return options;
	}

	public static OpenApiOptions AddNumericSchemas(this OpenApiOptions options)
	{
		options.AddDocumentTransformer<StringNumberUnionDocumentTransformer>();
		options.AddSchemaTransformer<SchemaRangeSchemaTransformer>();
		return options;
	}

	public static OpenApiOptions AddProblemDetailsDescriptions(this OpenApiOptions options)
	{
		options.AddDocumentTransformer<ProblemDetailsDescriptionDocumentTransformer>();
		return options;
	}

	public static OpenApiOptions AddRequiredNonNullableProperties(this OpenApiOptions options)
	{
		options.AddSchemaTransformer<RequiredNonNullableSchemaTransformer>();
		return options;
	}
}

