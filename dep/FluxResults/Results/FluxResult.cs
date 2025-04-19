using FluxResults.Abstractions.Results;
using FluxResults.Abstractions.TypedResults;
using FluxResults.TypedResults;

namespace FluxResults.Results;

public readonly struct FluxResult<T0> : IFluxResult
	where T0 : notnull
{
	private readonly T0? t0;
	private readonly IErrorTypedResult? error;
	public bool IsError => this.error is not null;

	private FluxResult(
		T0? value = default,
		IErrorTypedResult? error = null
	)
	{
		this.t0 = value;
		this.error = error;
	}

	public static implicit operator FluxResult<T0>(T0 t0) => new(value: t0);
	public static implicit operator FluxResult<T0>(Conflict error) => new(error: error);
	public static implicit operator FluxResult<T0>(NotFound error) => new(error: error);
	public static implicit operator FluxResult<T0>(ValidationError error) => new(error: error);
	public static implicit operator FluxResult<T0>(Unauthorized error) => new(error: error);
	public static implicit operator FluxResult<T0>(Forbidden error) => new(error: error);
	public static implicit operator FluxResult<T0>(UnexpectedError error) => new(error: error);

	public T0 Unwrap()
	{
		if (this.IsError)
		{
			throw new InvalidOperationException("Cannot unwrap a result that contains an error.");
		}

		return this.t0!;
	}

	public T0 UnwrapOr(T0 defaultValue)
	{
		if (this.IsError)
		{
			return defaultValue;
		}

		return this.t0!;
	}
}
