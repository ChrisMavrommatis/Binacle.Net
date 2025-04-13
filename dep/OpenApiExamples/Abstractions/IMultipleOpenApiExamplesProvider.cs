namespace OpenApiExamples.Abstractions;

public interface IMultipleOpenApiExamplesProvider
{
	IEnumerable<IOpenApiExample> GetExamples();
}

public interface IMultipleOpenApiExamplesProvider<out TModel> : IMultipleOpenApiExamplesProvider
	where TModel : class
{
	new IEnumerable<IOpenApiExample<TModel>> GetExamples();
	IEnumerable<IOpenApiExample> IMultipleOpenApiExamplesProvider.GetExamples() => this.GetExamples();
}
