namespace ChrisMavrommatis.Results.Unions;

public readonly struct OneOf<T0, T1> : IOneOf
{
	private readonly OneOfValue _enum;
	private readonly T0 _t0;
	private readonly T1 _t1;

	private OneOf(
		OneOfValue oneOfValue,
		T0 t0 = default,
		T1 t1 = default
		)
	{
		_enum = oneOfValue;
		_t0 = t0;
		_t1 = t1;
	}

	public bool Is<TResult>()
	{
		return typeof(TResult) == typeof(T0) && _enum == OneOfValue.T0
			|| typeof(TResult) == typeof(T1) && _enum == OneOfValue.T1;
	}
	
	public object GetValue()
	{
		return _enum switch
		{
			OneOfValue.T0 => _t0!,
			OneOfValue.T1 => _t1!,
			_ => throw new InvalidOperationException($"enum {_enum} is not a valid state for ResultOneOf<T0, T1>")
		};
	}

	public TResult GetValue<TResult>()
	{
		return _enum switch
		{
			OneOfValue.T0 => (TResult)(object)_t0!,
			OneOfValue.T1 => (TResult)(object)_t1!,
			_ => throw new InvalidOperationException($"enum {_enum} is not a valid state for ResultOneOf<T0, T1>")
		};
	}

	public TResult Unwrap<TResult>(
		Func<T0, TResult> f0,
		Func<T1, TResult> f1
		)
	{
		return _enum switch
		{
			OneOfValue.T0 => f0(_t0),
			OneOfValue.T1 => f1(_t1),
			_ => throw new InvalidOperationException($"enum {_enum} is not a valid state for ResultOneOf<T0, T1>")
		};

	}

	public static implicit operator OneOf<T0, T1>(T0 t0) => new OneOf<T0, T1>(OneOfValue.T0, t0: t0);
	public static implicit operator OneOf<T0, T1>(T1 t1) => new OneOf<T0, T1>(OneOfValue.T1, t1: t1);


}
