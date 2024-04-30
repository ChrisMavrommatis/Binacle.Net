using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace ChrisMavrommatis.SwaggerExamples;

public abstract class MultipleSwaggerExamplesProvider<T> : IMultipleSwaggerExamplesProvider<T>
	where T : class
{
	public abstract IEnumerable<ISwaggerExample<T>> GetExamples();

	IEnumerable<ISwaggerExample> IMultipleSwaggerExamplesProvider.GetExamples() => this.GetExamples();
}

