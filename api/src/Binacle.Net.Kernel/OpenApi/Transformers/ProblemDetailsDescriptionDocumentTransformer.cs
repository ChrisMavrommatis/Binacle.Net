using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.OpenApi.Transformers;

// Describes the framework's ProblemDetails / HttpValidationProblemDetails schemas, which no contract owns, so they
// cannot carry a [Description]. The wording is the RFC 7807 standard — not this API's domain — so it lives in the
// Kernel as generic, reusable infrastructure. Document-only.
internal class ProblemDetailsDescriptionDocumentTransformer : IOpenApiDocumentTransformer
{
	private const string ProblemDetails = "An error response in RFC 7807 problem-details format.";
	private const string ValidationProblemDetails =
		"An RFC 7807 problem-details response that also lists per-field validation errors.";

	private static readonly Dictionary<string, string> FieldDescriptions = new(StringComparer.Ordinal)
	{
		["type"] = "A URI reference identifying the problem type (RFC 7807).",
		["title"] = "A short, human-readable summary of the problem type (RFC 7807).",
		["status"] = "The HTTP status code for this problem (RFC 7807).",
		["detail"] = "A human-readable explanation specific to this occurrence (RFC 7807).",
		["instance"] = "A URI reference identifying the specific occurrence of the problem (RFC 7807).",
		["errors"] = "Validation errors, keyed by the field that failed.",
	};

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

		Describe(schemas, "ProblemDetails", ProblemDetails);
		Describe(schemas, "HttpValidationProblemDetails", ValidationProblemDetails);
		return Task.CompletedTask;
	}

	private static void Describe(IDictionary<string, IOpenApiSchema> schemas, string schemaName, string schemaDescription)
	{
		if (!schemas.TryGetValue(schemaName, out var value) || value is not OpenApiSchema schema)
		{
			return;
		}

		schema.Description = schemaDescription;

		if (schema.Properties is null)
		{
			return;
		}

		foreach (var (propertyName, property) in schema.Properties)
		{
			if (property is OpenApiSchema concrete && FieldDescriptions.TryGetValue(propertyName, out var description))
			{
				concrete.Description = description;
			}
		}
	}
}
