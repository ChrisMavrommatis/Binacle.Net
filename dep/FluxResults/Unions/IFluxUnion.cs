namespace FluxResults.Unions;

public interface IFluxUnion
{
	object GetValue();
	Type GetValueType();
}

public static class FluxUnionExtensions
{
	public static bool Is<T>(this IFluxUnion union)
	{
		return typeof(T) == union.GetValueType();
	}

	public static T? As<T>(this IFluxUnion union)
	{
		if (union.Is<T>())
		{
			return (T)union.GetValue();
		}

		return default;
	}

	public static T Unwrap<T>(this IFluxUnion union)
	{
		if (!union.Is<T>())
		{
			throw new InvalidOperationException($"Invalid result type {union.GetValueType()} cannot be cast to {typeof(T)}");
		}

		return (T)union.GetValue();
	}

	public static bool TryGetValue<T>(this IFluxUnion union, out T? result)
	{
		if (union.Is<T>())
		{
			result = (T)union.GetValue();
			return true;
		}

		result = default;
		return false;
	}
}
