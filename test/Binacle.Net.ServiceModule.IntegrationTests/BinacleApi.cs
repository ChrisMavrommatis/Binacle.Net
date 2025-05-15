using System.Text.Json;
using Binacle.Net.ServiceModule.Domain;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.IntegrationTests;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

[assembly: AssemblyFixture(typeof(BinacleApi))]

namespace Binacle.Net.ServiceModule.IntegrationTests;

public sealed class BinacleApi : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
	public Domain.Accounts.Entities.Account Admin { get; private set; } = null!;
	public Domain.Accounts.Entities.Account User { get; private set; } = null!;
	public AccountCredentials ExistingAccountCredentials { get; private set; } = null!;
	public readonly Guid NonExistentId;

	public BinacleApi()
	{
		this.Client = this.CreateClient();

		this.JsonSerializerOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};
		this.NonExistentId = Guid.Parse("EF81C267-A003-44B8-AD89-4B48661C4AA5");
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// TODO: Run the tests with all modules enabled

		var preBuildConfigurationValues = new Dictionary<string, string?>
		{
			{ "Features:SERVICE_MODULE", bool.TrueString },
			{ "RateLimiter:Auth", "NoLimiter::0" }
		};
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(preBuildConfigurationValues)
			.Build();

		builder
			// Additional configuration files are present when running in test
			// Because the project is set up to include the feature file, along with the environment as well
			// This will cause the tests to add the additional test config file
			.UseEnvironment("Test")
			// This configuration is used during the creation of the application
			// (e.g. BEFORE WebApplication.CreateBuilder(args) is called in Program.cs).
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				// This overrides configuration settings that were added as part
				// of building the Host (e.g. calling WebApplication.CreateBuilder(args)).
				configurationBuilder.AddInMemoryCollection(preBuildConfigurationValues);
				//configurationBuilder.AddJsonFile();
			});


		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
			services.Configure<ServiceModuleOptions>(options =>
			{
				options.DefaultAdminAccount = "testadmin@test.binacle.net:B1n4cl3Adm!nT3st";
			});
		});
	}

	public HttpClient Client { get; init; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }

	public async ValueTask InitializeAsync()
	{
		var passwordService = this.Services.GetRequiredService<IPasswordService>();
		var options = this.Services.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdmin = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);

		var accountRepository = this.Services.GetRequiredService<IAccountRepository>();
		var adminResult = await accountRepository.GetByUsernameAsync(defaultAdmin.Username);
		this.Admin = adminResult.Unwrap<Domain.Accounts.Entities.Account>();

		this.ExistingAccountCredentials = new AccountCredentials(
			Guid.Parse("C68B22B5-E41F-4E1B-9FEB-50A5CD46D441"),
			"existinguser@test.binacle.net",
			"existinguser@test.binacle.net",
			"Ex1stingUs3rP@ssw0rd"
		);

		this.User = new Domain.Accounts.Entities.Account(
			this.ExistingAccountCredentials.Username,
			AccountRole.User,
			this.ExistingAccountCredentials.Email,
			AccountStatus.Active,
			new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
			this.ExistingAccountCredentials.Id
		);
		var usersPassword = passwordService.Create(this.ExistingAccountCredentials.Password);
		this.User.ChangePassword(usersPassword);

		await accountRepository.CreateAsync(this.User);
	}

	public override async ValueTask DisposeAsync()
	{
		var accountRepository = this.Services.GetRequiredService<IAccountRepository>();
		await accountRepository.DeleteAsync(this.Admin);
		await accountRepository.DeleteAsync(this.User);
		await base.DisposeAsync();
	}

	public async ValueTask EnsureAccountExists(AccountCredentials credentials)
	{
		var accountRepository = this.Services.GetRequiredService<IAccountRepository>();
		var passwordService = this.Services.GetRequiredService<IPasswordService>();

		var account = new Domain.Accounts.Entities.Account(
			credentials.Username,
			AccountRole.User,
			credentials.Email,
			AccountStatus.Active,
			new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
			credentials.Id
		);

		var usersPassword = passwordService.Create(credentials.Password);
		account.ChangePassword(usersPassword);
		await accountRepository.CreateAsync(account);
	}

	public async ValueTask EnsureAccountDoesNotExist(AccountCredentials credentials)
	{
		var accountRepository = this.Services.GetRequiredService<IAccountRepository>();
		var getResult = await accountRepository.GetByIdAsync(credentials.Id, allowDeleted: true);
		if (!getResult.TryGetValue<Domain.Accounts.Entities.Account>(out var account) || account is null)
		{
			return;
		}

		if (account.HasSubscription())
		{
			var subscriptionRepository = this.Services.GetRequiredService<ISubscriptionRepository>();
			var subscriptionResult = await subscriptionRepository.GetByIdAsync(account.SubscriptionId!.Value, allowDeleted: true);
			if (subscriptionResult.TryGetValue<Domain.Subscriptions.Entities.Subscription>(out var subscription) &&
			    subscription is not null)
			{
				await subscriptionRepository.DeleteAsync(subscription);
			}
			
		}
		await accountRepository.DeleteAsync(account);
		
		
	}

	public async ValueTask EnsureAccountExistsWithSubscription(AccountCredentialsWithSubscription credentials)
	{
		var accountRepository = this.Services.GetRequiredService<IAccountRepository>();
		var passwordService = this.Services.GetRequiredService<IPasswordService>();
		var subscriptionRepository = this.Services.GetRequiredService<ISubscriptionRepository>();

		var account = new Domain.Accounts.Entities.Account(
			credentials.Username,
			AccountRole.User,
			credentials.Email,
			AccountStatus.Active,
			new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
			credentials.Id
		);

		var usersPassword = passwordService.Create(credentials.Password);
		account.ChangePassword(usersPassword);

		var subscription = new Domain.Subscriptions.Entities.Subscription(
			account.Id,
			SubscriptionStatus.Active,
			SubscriptionType.Normal,
			new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
			credentials.SubscriptionId
		);
		
		account.SetSubscription(subscription);
		await accountRepository.CreateAsync(account);
		await subscriptionRepository.CreateAsync(subscription);
	}

	public async ValueTask EnsureAccountWithSubscriptionDoesNotExist(AccountCredentialsWithSubscription credentials)
	{
		var accountRepository = this.Services.GetRequiredService<IAccountRepository>();
		var subscriptionRepository = this.Services.GetRequiredService<ISubscriptionRepository>();
		var getResult = await accountRepository.GetByIdAsync(credentials.Id, allowDeleted: true);
		if (!getResult.TryGetValue<Domain.Accounts.Entities.Account>(out var account) || account is null)
		{
			return;
		}

		var subscriptionResult = await subscriptionRepository.GetByIdAsync(credentials.SubscriptionId, allowDeleted: true);
		if (subscriptionResult.TryGetValue<Domain.Subscriptions.Entities.Subscription>(out var subscription) &&
		    subscription is not null)
		{
			await subscriptionRepository.DeleteAsync(subscription);
		}

		await accountRepository.DeleteAsync(account);
	}
}
