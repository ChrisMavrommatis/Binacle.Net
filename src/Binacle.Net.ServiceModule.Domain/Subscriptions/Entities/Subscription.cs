using Binacle.Net.ServiceModule.Domain.Common;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;

namespace Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;

public class Subscription : AuditableEntity
{
	private Guid? accountId;
	public SubscriptionType Type { get; private set; }
	
	private SubscriptionStatus status;
	
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
		this.accountId = accountId;
		this.status = status;
		this.Type = type;
	}

	public bool IsActive()
	{
		return !this.IsDeleted
		       && this.status == SubscriptionStatus.Active;
	}
}

