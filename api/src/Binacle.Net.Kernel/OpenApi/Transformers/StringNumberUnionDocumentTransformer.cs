using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.OpenApi.Transformers;

// The web JSON defaults accept a number as a JSON number or a numeric string, and ASP.NET mirrors that in the
// schema as an `[integer, string]` (or `[number, string]`) union — which makes a generated SDK type the value as
// `int | string`. This collapses the union back to the numeric type across the whole document.
//
// Generic on purpose: it keys off the schema shape, never off property names, so it belongs in the Kernel. It is
// document-only — runtime parsing is untouched, so the server still accepts numeric strings and the response wire
// does not change.
internal class StringNumberUnionDocumentTransformer : IOpenApiDocumentTransformer
{
	public Task TransformAsync(
		OpenApiDocument document,
		OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var schemas = document.Components?.Schemas;
		if (schemas is null)
		{
			return Task.CompletedTask;
		}

		foreach (var schema in schemas.Values)
		{
			if (schema.Properties is null)
			{
				continue;
			}

			foreach (var property in schema.Properties.Values)
			{
				if (property is OpenApiSchema concrete)
				{
					CollapseStringNumberUnion(concrete);
				}
			}
		}

		return Task.CompletedTask;
	}

	// `[integer, string]` / `[number, string]` collapses to the numeric type; the string-form pattern goes with
	// the dropped string branch. A `null` branch is kept so nullable-but-required handling stays intact.
	private static void CollapseStringNumberUnion(OpenApiSchema schema)
	{
		var type = schema.Type;
		if (type is null || !HasType(type, JsonSchemaType.String))
		{
			return;
		}

		if (!HasType(type, JsonSchemaType.Integer) && !HasType(type, JsonSchemaType.Number))
		{
			return;
		}

		schema.Type = type.Value & ~JsonSchemaType.String;
		schema.Pattern = null;
	}

	private static bool HasType(JsonSchemaType? type, JsonSchemaType flag)
		=> type is not null && (type.Value & flag) == flag;
}
