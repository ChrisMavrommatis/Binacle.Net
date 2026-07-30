using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.Kernel.OpenApi.Transformers;

// For a type marked [OpenApiRequireNonNullable], lists every non-nullable serialized property in the schema's
// `required` set. Nullability is read from the CLR type (value types and non-nullable reference types), so it
// stays in step with the contract. Generic: no property names live in the Kernel.
internal class RequiredNonNullableSchemaTransformer : IOpenApiSchemaTransformer
{
	private static readonly NullabilityInfoContext NullabilityContext = new();

	public Task TransformAsync(
		OpenApiSchema schema,
		OpenApiSchemaTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var typeInfo = context.JsonTypeInfo;
		if (typeInfo is null
			|| !typeInfo.Type.IsDefined(typeof(OpenApiRequireNonNullableAttribute), inherit: true))
		{
			return Task.CompletedTask;
		}

		foreach (var property in typeInfo.Properties)
		{
			if (!IsNonNullable(property.PropertyType, property.AttributeProvider as PropertyInfo))
			{
				continue;
			}

			schema.Required ??= new HashSet<string>();
			schema.Required.Add(property.Name);
		}

		return Task.CompletedTask;
	}

	private static bool IsNonNullable(Type propertyType, PropertyInfo? member)
	{
		if (propertyType.IsValueType)
		{
			return Nullable.GetUnderlyingType(propertyType) is null;
		}

		return member is not null && NullabilityContext.Create(member).WriteState == NullabilityState.NotNull;
	}
}
