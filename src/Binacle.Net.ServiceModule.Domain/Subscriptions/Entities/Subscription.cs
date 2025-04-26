using Binacle.Net.ServiceModule.Domain.Common;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;

namespace Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;

public class Subscription : AuditableEntity
{
	public Guid AccountId { get; }
	public SubscriptionType Type { get; private set; }
	
	public SubscriptionStatus Status { get; private set; }
	
	public Subscription(
		Guid accountId,
		SubscriptionStatus status,
		SubscriptionType type,
		DateTimeOffset creationDate,
		Guid? id = null
	) 
		: base(
			id ?? Guid.CreateVersion7(),
			creationDate
		)
	{
		this.AccountId = accountId;
		this.Status = status;
		this.Type = type;
	}
	
	public void ChangeType(SubscriptionType newType)
	{
		this.Type = newType;
	}
	
	public void ChangeStatus(SubscriptionStatus newStatus)
	{
		this.Status = newStatus;
	}
	public bool IsActive()
	{
		return !this.IsDeleted
		       && this.Status == SubscriptionStatus.Active;
	}
}

