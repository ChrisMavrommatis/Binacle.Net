using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace ChrisMavrommatis.SwaggerExamples;

public abstract class SingleSwaggerExamplesProvider<T> : ISingleSwaggerExamplesProvider<T>
	where T : class
{
	public abstract T GetExample();

	object ISingleSwaggerExamplesProvider.GetExample() => this.GetExample();
}

