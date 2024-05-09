namespace ChrisMavrommatis.Results.Unions;

public readonly struct OneOf<T0, T1, T2> : IOneOf
{
	private readonly OneOfValue _enum;
	private readonly T0 _t0;
	private readonly T1 _t1;
	private readonly T2 _t2;

	private OneOf(
		OneOfValue oneOfValue, 
		T0 t0 = default, 
		T1 t1 = default, 
		T2 t2 = default
		)
	{
		_enum = oneOfValue;
		_t0 = t0;
		_t1 = t1;
		_t2 = t2;
	}

	public bool Is<TResultType>()
	{
		return typeof(TResultType) == typeof(T0) && _enum == OneOfValue.T0
			|| typeof(TResultType) == typeof(T1) && _enum == OneOfValue.T1
			|| typeof(TResultType) == typeof(T2) && _enum == OneOfValue.T2;
	}

	public object GetValue()
	{
		return _enum switch
		{
			OneOfValue.T0 => _t0!,
			OneOfValue.T1 => _t1!,
			OneOfValue.T2 => _t2!,
			_ => throw new InvalidOperationException($"enum {_enum} is not a valid state for ResultOneOf<T0, T1>")
		};
		
	}

	public TResult GetValue<TResult>()
	{
		return _enum switch
		{
			OneOfValue.T0 => (TResult)(object)_t0!,
			OneOfValue.T1 => (TResult)(object)_t1!,
			OneOfValue.T2 => (TResult)(object)_t2!,
			_ => throw new InvalidOperationException($"enum {_enum} is not a valid state for ResultOneOf<T0, T1>")
		};
	}


	public TResult Unwrap<TResult>(
		Func<T0, TResult> f0, 
		Func<T1, TResult> f1, 
		Func<T2, TResult> f2
		)
	{
		return _enum switch
		{
			OneOfValue.T0 => f0(_t0),
			OneOfValue.T1 => f1(_t1),
			OneOfValue.T2 => f2(_t2),
			_ => throw new InvalidOperationException()
		};

	}

	public static implicit operator OneOf<T0, T1, T2>(T0 t0) => new OneOf<T0, T1, T2>(OneOfValue.T0, t0: t0);
	public static implicit operator OneOf<T0, T1, T2>(T1 t1) => new	OneOf<T0, T1, T2>(OneOfValue.T1, t1: t1);
	public static implicit operator OneOf<T0, T1, T2>(T2 t2) => new OneOf<T0, T1, T2>(OneOfValue.T2, t2: t2);
}
