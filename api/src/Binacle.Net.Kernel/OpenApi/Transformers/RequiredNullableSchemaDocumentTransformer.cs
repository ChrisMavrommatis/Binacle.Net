using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.OpenApi.Transformers;

// A property typed `SomeEnum?` but marked required renders as `oneOf: [null, $ref]` — the generator
// describes the C# type, which allows null, while the validator rejects it. A client generated from
// that schema would send null and get a 422. The nullable C# type is deliberate: it is what lets the
// validator answer with the list of valid values instead of a raw deserializer error, so the schema
// is what has to give.
//
// This runs at document level because a schema transformer cannot fix it: for a nullable enum the
// schema transformer is handed the enum's own schema (the one hoisted into components), never the
// property schema, and the `oneOf` wrapper is added after transformers run.
internal class RequiredNullableSchemaDocumentTransformer : IOpenApiDocumentTransformer
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
			RemoveNullFromRequiredProperties(schema);
		}

		return Task.CompletedTask;
	}

	private static void RemoveNullFromRequiredProperties(IOpenApiSchema schema)
	{
		if (schema.Properties is null || schema.Required is null)
		{
			return;
		}

		foreach (var propertyName in schema.Required)
		{
			if (!schema.Properties.TryGetValue(propertyName, out var property))
			{
				continue;
			}

			if (property is not OpenApiSchema concreteProperty)
			{
				continue;
			}

			var branches = concreteProperty.OneOf;
			if (branches is null || branches.Count != 2)
			{
				continue;
			}

			var nullBranch = branches.FirstOrDefault(branch => branch.Type == JsonSchemaType.Null);
			if (nullBranch is null)
			{
				continue;
			}

			// Collapse `oneOf: [null, X]` down to X — a required property cannot be null here.
			var valueBranch = branches.First(branch => !ReferenceEquals(branch, nullBranch));
			schema.Properties[propertyName] = valueBranch;
		}
	}
}
