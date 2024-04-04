namespace ChrisMavrommatis.SwaggerExamples.Abstractions;

internal interface IMultipleSwaggerExamplesProvider<T> : IMultipleSwaggerExamplesProvider
	where T : class
{
	IEnumerable<ISwaggerExample<T>> GetExamples();
}
internal interface IMultipleSwaggerExamplesProvider
{
	IEnumerable<ISwaggerExample> GetExamples();
}
