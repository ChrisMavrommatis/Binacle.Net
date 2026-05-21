using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.OpenApi;

internal class EnumStringsSchemaTransformer : IOpenApiSchemaTransformer
{
	public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
		CancellationToken cancellationToken)
	{
		var propertyType = context?.JsonPropertyInfo?.PropertyType;
		if (propertyType is null)
		{
			return Task.CompletedTask;
		}

		var underlyingPropertyType = Nullable.GetUnderlyingType(propertyType)
		                             ?? propertyType;
		
		if (!underlyingPropertyType.IsEnum)
			return Task.CompletedTask;
		
		
		var converterType = context?.JsonPropertyInfo?.CustomConverter?.GetType();
		if (converterType == typeof(JsonStringEnumConverter))
		{
			schema.Type = JsonSchemaType.String;
			return Task.CompletedTask;
		}

			
		// TODO: if property required then remove nullable and from name
		if ((converterType?.IsGenericType ?? false) && converterType?.GetGenericTypeDefinition() == typeof(JsonStringNullableEnumConverter<>))
		{
			schema.Type = JsonSchemaType.String;
			schema.Enum = this.GetEnumOptions(underlyingPropertyType);
			return Task.CompletedTask;
		}
		
		if (converterType == typeof(JsonStringNullableEnumConverter))
		{
			schema.Type = JsonSchemaType.String;
			schema.Enum = this.GetEnumOptions(underlyingPropertyType);
			return Task.CompletedTask;
		}
		return Task.CompletedTask;

	}

	private IList<JsonNode> GetEnumOptions(Type enumType)
	{
		var enumValues = Enum.GetValues(enumType).Cast<object>();
		
		var enumDesc = enumValues.Select(value =>
		{
			var field = enumType.GetField(value.ToString()!);

			return new
			{
				Value = (int)value,
				field?.Name
			};
		});
		
		var result = new List<JsonNode>();
		foreach (var item in enumDesc)
		{
			result.Add(JsonValue.Create(item.Name)!);
		}

		return result;
	}
}
