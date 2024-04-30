namespace ChrisMavrommatis.SwaggerExamples.Abstractions;

public interface ISwaggerExample<T> : ISwaggerExample
{
	string Key { get; }
	string Summary { get; }
	string Description { get; }
	T Value { get; }
}


public interface ISwaggerExample
{
	string Key { get; }
	string Summary { get; }
	string Description { get; }
	object Value { get; }
}
