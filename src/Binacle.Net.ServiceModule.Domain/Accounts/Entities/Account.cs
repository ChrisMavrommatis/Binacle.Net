using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Common;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Domain.Accounts.Entities;

public class Account : AuditableEntity
{
	public string Username { get; private set; }
	public AccountRole Role { get; private set; }
	public string Email { get; private set; }
	public AccountStatus Status { get; private set; }

	public Password? Password { get; private set; }
	public Guid SecurityStamp { get; private set; }
	
	public Guid? SubscriptionId { get; private set; }
	
	public Account(
		string username,
		AccountRole role,
		string email,
		AccountStatus status,
		DateTimeOffset creationDate,
		Guid? id = null
	)
		: base(
			id ?? Guid.CreateVersion7(),
			creationDate
		)
	{
		this.Username = username;
		this.Role = role;
		this.Email = email;
		this.Status = status;
		this.SecurityStamp = Guid.Empty;
	}
	
	public bool HasPassword()
	{
		return this.Password is not null
			&& this.SecurityStamp != Guid.Empty;
	}

	public void ChangePassword(Password password)
	{
		this.Password = password;
		this.SecurityStamp = Guid.NewGuid();
	}
	
	public void ChangeUsername(string newUsername)
	{
		this.Username = newUsername;
	}
	
	public void ChangeEmail(string newEmail)
	{
		this.Email = newEmail;
	}

	public void ChangeRole(AccountRole newRole)
	{
		this.Role = newRole;
	}
	
	public void ChangeStatus(AccountStatus newStatus)
	{
		this.Status = newStatus;
	}

	public FluxUnion<Success, Conflict> SetSubscription(Subscription subscription)
	{
		if (this.SubscriptionId.HasValue)
		{
			return TypedResult.Conflict;
		}

		this.SubscriptionId = subscription.Id;

		return TypedResult.Success;
	}

	public FluxUnion<Success, NotFound> RemoveSubscription()
	{
		if (!this.SubscriptionId.HasValue)
		{
			return TypedResult.NotFound;
		}

		this.SubscriptionId = null;
		return TypedResult.Success;
	}
	
	public bool HasSubscription()
	{
		return this.SubscriptionId.HasValue
			&& this.SubscriptionId.Value != Guid.Empty;
	}

	public bool IsActive()
	{
		return !this.IsDeleted
		       && this.Status == AccountStatus.Active;
	}

	
}


