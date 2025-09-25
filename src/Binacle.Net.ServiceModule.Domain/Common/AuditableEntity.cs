using Binacle.Net.ServiceModule.Domain.Common.Interfaces;

namespace Binacle.Net.ServiceModule.Domain.Common;

public abstract class AuditableEntity : Entity, IWithCreatedTime, ISoftDeletable
{
	public DateTimeOffset CreatedAtUtc { get; private set; }
	public bool IsDeleted { get; private set; }
	public DateTimeOffset? DeletedAtUtc { get; private set; }

	protected AuditableEntity(
		Guid id,
		DateTimeOffset creationDate,
		bool isDeleted,
		DateTimeOffset? deletedAtUtc = null
		) : this(id, creationDate)
	{
		if (isDeleted && !deletedAtUtc.HasValue)
		{
			throw new InvalidOperationException($"Cannot construct entity of type {this.GetType().Name}. 'IsDeleted' is set to 'true' but no 'DeletedAtUtc' was provided");
		}

		this.IsDeleted = isDeleted;
		this.DeletedAtUtc = deletedAtUtc;
	}
	protected AuditableEntity(Guid id, DateTimeOffset creationDate) : base(id)
	{
		this.CreatedAtUtc = creationDate;
		this.IsDeleted = false;
	}

	public void SoftDelete(DateTimeOffset deletionDate)
	{
		this.IsDeleted = true;
		this.DeletedAtUtc = deletionDate;
	}
}
