using Binacle.Net.ServiceModule.Domain.Common.Interfaces;

namespace Binacle.Net.ServiceModule.Domain.Common;

public abstract class AuditableEntity : Entity, IWithCreatedTime, ISoftDeletable
{
	public DateTimeOffset CreatedAtUtc { get; private set; }
	public bool IsDeleted { get; private set; }
	public DateTimeOffset? DeletedAtUtc { get; private set; }

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
