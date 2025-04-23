namespace FluxResults.Unions;

public readonly struct FluxUnion<T0, T1, T2, T3> : IFluxUnion
	where T0 : notnull
	where T1 : notnull
	where T2 : notnull
	where T3 : notnull
{
	private enum Flux
	{
		T0,
		T1,
		T2,
		T3
	}

	private readonly Flux flux;
	private readonly T0? t0;
	private readonly T1? t1;
	private readonly T2? t2;
	private readonly T3? t3;

	private FluxUnion(
		Flux flux,
		T0? t0 = default,
		T1? t1 = default,
		T2? t2 = default,
		T3? t3 = default
	)
	{
		this.flux = flux;
		this.t0 = t0;
		this.t1 = t1;
		this.t2 = t2;
		this.t3 = t3;
	}

	public static implicit operator FluxUnion<T0, T1, T2, T3>(T0 t0) => new(Flux.T0, t0: t0);
	public static implicit operator FluxUnion<T0, T1, T2, T3>(T1 t1) => new(Flux.T1, t1: t1);
	public static implicit operator FluxUnion<T0, T1, T2, T3>(T2 t2) => new(Flux.T2, t2: t2);
	public static implicit operator FluxUnion<T0, T1, T2, T3>(T3 t3) => new(Flux.T3, t3: t3);

	public object GetValue()
	{
		return this.flux switch
		{
			Flux.T0 => this.t0!,
			Flux.T1 => this.t1!,
			Flux.T2 => this.t2!,
			Flux.T3 => this.t3!,
			_ => throw new InvalidOperationException("Invalid result value.")
		};
	}

	public Type GetValueType()
	{
		return this.flux switch
		{
			Flux.T0 => typeof(T0),
			Flux.T1 => typeof(T1),
			Flux.T2 => typeof(T2),
			Flux.T3 => typeof(T3),
		};
	}
	
	public TResult Match<TResult>(
		Func<T0, TResult> t0Func,
		Func<T1, TResult> t1Func,
		Func<T2, TResult> t2Func,
		Func<T3, TResult> t3Func
	)
	{
		return this.flux switch
		{
			Flux.T0 => t0Func(this.t0!),
			Flux.T1 => t1Func(this.t1!),
			Flux.T2 => t2Func(this.t2!),
			Flux.T3 => t3Func(this.t3!),
			_ => throw new InvalidOperationException("Invalid result value.")
		};
	}
	
}
