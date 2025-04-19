using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Common;
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

	public string? PasswordHash { get; private set; }
	public Guid SecurityStamp { get; private set; }
	
	private Guid? subscriptionId;
	
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
		return !string.IsNullOrEmpty(this.PasswordHash)
			&& this.SecurityStamp != Guid.Empty;
	}

	public void ChangePassword(string passwordHash)
	{
		this.PasswordHash = passwordHash;
		this.SecurityStamp = Guid.NewGuid();
	}

	public FluxUnion<Success, Conflict> SetSubscription(Subscription subscription)
	{
		if (this.subscriptionId.HasValue)
		{
			return TypedResult.Conflict;
		}

		this.subscriptionId = subscription.Id;

		return TypedResult.Success;
	}
	
	public bool HasSubscription()
	{
		return this.subscriptionId.HasValue
			&& this.subscriptionId.Value != Guid.Empty;
	}

	public bool IsActive()
	{
		return !this.IsDeleted
		       && this.Status == AccountStatus.Active;
	}

	
}


