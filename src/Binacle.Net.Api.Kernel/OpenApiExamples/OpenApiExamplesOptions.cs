using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OpenApiExamples.Abstractions;

namespace OpenApiExamples;

public class OpenApiExamplesOptions
{
	internal Action<JsonExamplesFormatterOptions>? JsonFormatterConfigurator;
	public Dictionary<string, IOpenApiExamplesFormatter> Formatters { get; } = new();
	
	public OpenApiExamplesOptions ConfigureJsonFormatter(
		Action<JsonExamplesFormatterOptions> configure)
	{
		this.JsonFormatterConfigurator = configure;
		return this;
	}
	
}

public class JsonExamplesFormatterOptions
{
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();
}

public class BinacleOpenApiExample<T> : IOpenApiExample<T>
{
	public string Key { get; set; }

	public string Summary { get; set; }

	public string Description { get; set; }

	public T Value { get; set; }

	object IOpenApiExample.Value => this.Value;
}

public static class BinacleOpenApiExample
{
	public static BinacleOpenApiExample<T> Create<T>(string key, T value)
	{
		return new BinacleOpenApiExample<T>
		{
			Key = key,
			Value = value
		};
	}
	public static BinacleOpenApiExample<T> Create<T>(string key, string summary, T value)
	{
		return new BinacleOpenApiExample<T>
		{
			Key = key,
			Summary = summary,
			Value = value
		};
	}

	public static BinacleOpenApiExample<T> Create<T>(string key, string summary, string description, T value)
	{
		return new BinacleOpenApiExample<T>
		{
			Key = key,
			Summary = summary,
			Description = description,
			Value = value
		};
	}
}

public abstract class MultipleOpenApiExamplesProvider<T> : IMultipleOpenApiExamplesProvider<T>
	where T : class
{
	public abstract IEnumerable<IOpenApiExample<T>> GetExamples();

	IEnumerable<IOpenApiExample> IMultipleOpenApiExamplesProvider.GetExamples() => this.GetExamples();
}

public abstract class SingleOpenApiExamplesProvider<T> : ISingleOpenApiExamplesProvider<T>
	where T : class
{
	public abstract IOpenApiExample<T> GetExample();

	IOpenApiExample ISingleOpenApiExamplesProvider.GetExample() => this.GetExample();
}
