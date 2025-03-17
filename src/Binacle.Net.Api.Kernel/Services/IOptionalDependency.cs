using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Api.Services;

public interface IOptionalDependency<T>
{
	T? Value { get; }
}

public class OptionalDependency<T> : IOptionalDependency<T>
{
	public T? Value { get; set; }

	public OptionalDependency(IServiceProvider serviceProvider)
	{
		Value = serviceProvider.GetService<T>();
	}
}
