using System.Globalization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.Kernel.OpenApi.Transformers;

// Applies OpenApiSchemaRangeAttribute from a property onto its schema. Generic: it reads whatever attribute the
// contract declares, so the Kernel carries no property names or domain ranges of its own.
internal class SchemaRangeSchemaTransformer : IOpenApiSchemaTransformer
{
	public Task TransformAsync(
		OpenApiSchema schema,
		OpenApiSchemaTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		var attribute = context.JsonPropertyInfo?.AttributeProvider?
			.GetCustomAttributes(typeof(OpenApiSchemaRangeAttribute), inherit: false)
			.OfType<OpenApiSchemaRangeAttribute>()
			.FirstOrDefault();

		if (attribute is null)
		{
			return Task.CompletedTask;
		}

		if (!double.IsNaN(attribute.Minimum))
		{
			schema.Minimum = Format(attribute.Minimum);
		}

		if (!double.IsNaN(attribute.Maximum))
		{
			schema.Maximum = Format(attribute.Maximum);
		}

		return Task.CompletedTask;
	}

	private static string Format(double value)
		=> value.ToString("R", CultureInfo.InvariantCulture);
}
