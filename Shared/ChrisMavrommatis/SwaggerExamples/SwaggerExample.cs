using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace ChrisMavrommatis.SwaggerExamples;

public class SwaggerExample<T> : ISwaggerExample<T>
{
	public string Key { get; set; }

	public string Summary { get; set; }

	public string Description { get; set; }

	public T Value { get; set; }

	object ISwaggerExample.Value => this.Value;
}

public static class SwaggerExample
{
	public static SwaggerExample<T> Create<T>(string key, T value)
	{
		return new SwaggerExample<T>
		{
			Key = key,
			Value = value
		};
	}
	public static SwaggerExample<T> Create<T>(string key, string summary, T value)
	{
		return new SwaggerExample<T>
		{
			Key = key,
			Summary = summary,
			Value = value
		};
	}

	public static SwaggerExample<T> Create<T>(string key, string summary, string description, T value)
	{
		return new SwaggerExample<T>
		{
			Key = key,
			Summary = summary,
			Description = description,
			Value = value
		};
	}
}
