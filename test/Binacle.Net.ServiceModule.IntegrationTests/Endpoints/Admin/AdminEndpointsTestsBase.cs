using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin;

public abstract partial class AdminEndpointsTestsBase :  IAsyncLifetime
{
	protected readonly BinacleApiAsAServiceFactory Sut;
	protected readonly Domain.Accounts.Entities.Account AdminAccount;
	protected readonly Domain.Accounts.Entities.Account UserAccount;
	
	
	public AdminEndpointsTestsBase(BinacleApiAsAServiceFactory sut)
	{
		this.Sut = sut;
		
		var passwordService = this.Sut.Services.GetRequiredService<IPasswordService>();
		var options = this.Sut.Services.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdmin = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);

		this.AdminAccount = new Domain.Accounts.Entities.Account(
			defaultAdmin.Username,
			AccountRole.Admin,
			defaultAdmin.Email,
			AccountStatus.Active,
			new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
			Guid.Parse("6B54DFF4-8130-4572-A21A-A305F9018FFF")
		);

		var adminsPassword = passwordService.Create(defaultAdmin.Password);
		this.AdminAccount.ChangePassword(adminsPassword);
		
		this.UserAccount =new Domain.Accounts.Entities.Account(
			"testuser@binacle.net",
			AccountRole.User,
			"testuser@binacle.net",
			AccountStatus.Active,
			new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
			Guid.Parse("C68B22B5-E41F-4E1B-9FEB-50A5CD46D441")
		);
		var usersPassword = passwordService.Create("T3stUs3rsP@ssw0rd");
		this.UserAccount.ChangePassword(usersPassword);
	}
	
	protected async Task EnsureAccountExists(AccountCredentials accountCredentials)
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var request = new AccountCreateRequest()
		{
			Username = accountCredentials.Username,
			Email = accountCredentials.Email,
			Password = accountCredentials.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync("/api/admin/account", request, this.Sut.JsonSerializerOptions);
		if (response.StatusCode != HttpStatusCode.Created &&
		    response.StatusCode != HttpStatusCode.Conflict)
		{
			throw new ApplicationException($"Error while creating account {accountCredentials.Username}. Response status code was {response.StatusCode}");
		}

		if (response.StatusCode == HttpStatusCode.Created)
		{
			accountCredentials.Id = GetCreatedId(response);
		}
	}
	
	protected async Task EnsureAccountHasSubscription(AccountCredentials accountCredentials)
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};

		var response = await this.Sut.Client.PostAsJsonAsync($"/api/admin/account/{accountCredentials.Id.ToString()}/subscription/", request, this.Sut.JsonSerializerOptions);
		if (response.StatusCode != HttpStatusCode.Created &&
		    response.StatusCode != HttpStatusCode.Conflict)
		{
			throw new ApplicationException($"Error while creating subscription for {accountCredentials.Username}. Response status code was {response.StatusCode}");
		}
	}

	protected Guid GetCreatedId(HttpResponseMessage response)
	{
		var location = response.Headers.Location!.ToString();
		var parts = location.Split(["/"], StringSplitOptions.RemoveEmptyEntries);
		var id = Guid.Parse(parts.Last());
		return id;
	}
	
	protected async Task EnsureAccountDoesNotExist(AccountCredentials accountCredentials)
	{
		if (accountCredentials.Id is null)
		{
			// TODO: In create can create users
			return;
		}
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var response = await this.Sut.Client.DeleteAsync($"/api/admin/account/{accountCredentials.Id}");

		if (response.StatusCode != HttpStatusCode.NoContent &&
		    response.StatusCode != HttpStatusCode.NotFound)
		{
			throw new ApplicationException($"Error while deleting Account {accountCredentials.Username}. Response status code was {response.StatusCode}");
		}
	}
	
	protected async Task EnsureAccountDoesNotHaveSubscription(AccountCredentials accountCredentials)
	{
		if (accountCredentials.Id is null)
		{
			// TODO: In create can create users
			return;
		}
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var response = await this.Sut.Client.DeleteAsync($"/api/admin/account/{accountCredentials.Id}/subscription");

		if (response.StatusCode != HttpStatusCode.NoContent &&
		    response.StatusCode != HttpStatusCode.NotFound)
		{
			throw new ApplicationException($"Error while deleting Subscription for {accountCredentials.Username}. Response status code was {response.StatusCode}");
		}
	}
	
	public virtual ValueTask InitializeAsync()
	{
		return ValueTask.CompletedTask;
	}

	public virtual ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}
