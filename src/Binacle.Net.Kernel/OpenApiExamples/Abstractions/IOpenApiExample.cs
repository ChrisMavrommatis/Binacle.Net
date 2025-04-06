namespace OpenApiExamples.Abstractions;

public interface IOpenApiExample<out T> : IOpenApiExample
{
	new T Value { get;  }
}

public interface IOpenApiExample
{
	string Key { get; }
	string? Summary { get; }
	string? Description { get; }
	object Value { get; }
}
