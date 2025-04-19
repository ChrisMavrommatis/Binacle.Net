namespace Binacle.Net.ServiceModule.Domain.Common;

public abstract class Entity
{
	public Guid Id { get; }

	protected Entity(Guid id)
	{
		this.Id = id;
	}

	public override bool Equals(object? obj)
	{
		if (obj is not Entity other)
			return false;

		return this.Id.Equals(other.Id);
	}

	public override int GetHashCode()
	{
		return this.Id.GetHashCode();
	}
}
