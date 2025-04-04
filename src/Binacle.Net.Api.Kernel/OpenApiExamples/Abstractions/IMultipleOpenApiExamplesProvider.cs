namespace OpenApiExamples.Abstractions;

public interface IMultipleOpenApiExamplesProvider
{
	IEnumerable<IOpenApiExample> GetExamples();
}

public interface IMultipleOpenApiExamplesProvider<TModel> : IMultipleOpenApiExamplesProvider
	where TModel : class
{
	IEnumerable<IOpenApiExample<TModel>> GetExamples();
}
