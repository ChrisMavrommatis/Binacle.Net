namespace Binacle.Net.ServiceModule.Domain.Common;

public abstract class ValueObject
{
	protected abstract IEnumerable<object> GetAtomicValues();

	public sealed override bool Equals(object? obj)
	{
		if (obj is not ValueObject other)
			return false;

		return this.GetAtomicValues().SequenceEqual(other.GetAtomicValues());
	}

	public sealed override int GetHashCode()
	{
		return this.GetAtomicValues()
			.Select(x => x?.GetHashCode() ?? 0)
			.Aggregate((x, y) => x ^ y);
	}
}
