namespace ChrisMavrommatis.SwaggerExamples.Abstractions;

internal interface ISingleSwaggerExamplesProvider<TModel> : ISingleSwaggerExamplesProvider
	where TModel : class
{
	TModel GetExample();
}

internal interface ISingleSwaggerExamplesProvider
{
	object GetExample();
}
