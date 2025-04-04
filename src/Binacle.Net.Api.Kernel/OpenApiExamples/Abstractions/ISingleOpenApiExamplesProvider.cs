namespace OpenApiExamples.Abstractions;


public interface ISingleOpenApiExamplesProvider
{
	IOpenApiExample GetExample();
}

public interface ISingleOpenApiExamplesProvider<TModel> : ISingleOpenApiExamplesProvider
	where TModel : class
{
	IOpenApiExample<TModel> GetExample();
}
