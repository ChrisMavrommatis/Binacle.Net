namespace OpenApiExamples.Abstractions;


public interface ISingleOpenApiExamplesProvider
{
	IOpenApiExample GetExample();
}

public interface ISingleOpenApiExamplesProvider<out TModel> : ISingleOpenApiExamplesProvider
	where TModel : class
{
	new IOpenApiExample<TModel> GetExample();
	IOpenApiExample ISingleOpenApiExamplesProvider.GetExample() => this.GetExample();
}
