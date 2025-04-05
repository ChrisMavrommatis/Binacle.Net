using OpenApiExamples.Abstractions;

namespace OpenApiExamples;

internal class OpenApiExample<T> : IOpenApiExample<T>
{
	
	public required string Key { get; init; }

	public string? Summary { get; init; }

	public string? Description { get; init; }

	public required T Value { get; init; }

	object IOpenApiExample.Value => this.Value!;
}

public static class OpenApiExample
{
	public static IOpenApiExample<T> Create<T>(string key, T value)
	{
		return new OpenApiExample<T>
		{
			Key = key,
			Value = value
		};
	}
	public static IOpenApiExample<T> Create<T>(string key, string summary, T value)
	{
		return new OpenApiExample<T>
		{
			Key = key,
			Summary = summary,
			Value = value
		};
	}

	public static IOpenApiExample<T> Create<T>(string key, string summary, string description, T value)
	{
		return new OpenApiExample<T>
		{
			Key = key,
			Summary = summary,
			Description = description,
			Value = value
		};
	}
}
