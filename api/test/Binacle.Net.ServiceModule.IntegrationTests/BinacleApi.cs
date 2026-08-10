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

		var infra = ResolveTestInfrastructure();

		var preBuildConfigurationValues = new Dictionary<string, string?>
		{
			{ "Features:SERVICE_MODULE", bool.TrueString },
			{ "RateLimiter:AuthToken", "NoLimiter::0" },
			// Only the selected backend's connection string is set, so Setup.AddInfrastructure registers exactly it.
			{ infra.ConfigKey, infra.ConnectionString },
			{ "JwtAuth:Issuer", "ForTestsOnly"},
			{ "JwtAuth:Audience", "ForTestsOnly" },	
			{ "JwtAuth:TokenSecret", "SecretKeyForTestsOnly_paddedTo70Plus_paddedTo70Plus_paddedTo70Plus_paddedTo70Plus"},
			{ "JwtAuth:ExpirationInSeconds", "3600"}
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(preBuildConfigurationValues)
			.Build();

		builder
			// Picks up the Test-environment config files the project ships alongside the app's own.
			.UseEnvironment("Test")
			// Seeds the builder: applied before Program.cs reaches WebApplication.CreateBuilder.
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				// Applied after the host built its own configuration, so these values win.
				configurationBuilder.AddInMemoryCollection(preBuildConfigurationValues);
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

	// Infra is chosen the way production chooses it — by which connection string is set (prod env names).
	// The only addition is an explicit SQLite fallback so a bare `dotnet test` runs with no external service;
	// CI sets one env var per matrix leg. The choice is logged by the caller so it is never silent.
	private static (string ConfigKey, string ConnectionString) ResolveTestInfrastructure()
	{
		var testInfra = Environment.GetEnvironmentVariable("BINACLE_TEST_INFRA");
		if (testInfra == "AzureStorage")
		{
			var azureConnectionStringOverride = Environment.GetEnvironmentVariable("AZURESTORAGE_CONNECTION_STRING");
			var defaultAzureStorageConnectionString = "UseDevelopmentStorage=true";
			LogChoiceToConsole("Azure Storage", azureConnectionStringOverride is not null);
			return ("ConnectionStrings:AzureStorage",
				azureConnectionStringOverride ?? defaultAzureStorageConnectionString);
		}

		if (testInfra == "Postgres")
		{
			var postgresConnectionStringOverride = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
			var defaultPostgresConnectionString = "Host=localhost;Port=5432;Database=binacle_net;Username=appuser;Password=Pl3@s3UseASt0ngP@ssw0rdN0tL!k3Th!s0n3";
			LogChoiceToConsole("Postgres", postgresConnectionStringOverride is not null);
			return ("ConnectionStrings:Postgres", postgresConnectionStringOverride ??  defaultPostgresConnectionString);
		}

		if (testInfra == "Sqlite")
		{
			var sqliteConnectionStringOverride = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING");
			var defaultSqliteConnectionString = "DataSource=binacle-net-service.test.db;";
			LogChoiceToConsole("SQLite", sqliteConnectionStringOverride is not null);
			return ("ConnectionStrings:Sqlite", sqliteConnectionStringOverride ?? defaultSqliteConnectionString);
		}
		LogChoiceToConsole("SQLite (Fallback)", false);
		return ("ConnectionStrings:Sqlite", "DataSource=binacle-net-service.test.db;");
	}
	
	private static void LogChoiceToConsole(string name, bool isOverride)
	{
		Console.WriteLine($"[ServiceModule.IntegrationTests] infra backend: {name} {(isOverride ? "(override)" : "(default)")}");
	}

	public HttpClient Client { get; init; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }

	public async ValueTask InitializeAsync()
	{
		// A fresh DI scope per helper call: the DB repositories are scoped and a SQLite connection is not
		// thread-safe, so resolving from the root provider shares one connection across parallel tests.
		using var scope = this.Services.CreateScope();
		var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
		var options = scope.ServiceProvider.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdmin = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);

		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
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
		using var scope = this.Services.CreateScope();
		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
		await accountRepository.DeleteAsync(this.Admin);
		await accountRepository.DeleteAsync(this.User);
		await base.DisposeAsync();
	}

	public async ValueTask EnsureAccountExists(AccountCredentials credentials)
	{
		using var scope = this.Services.CreateScope();
		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
		var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

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

	public async ValueTask EnsureAccountWithUsernameDoesNotExist(string username)
	{
		using var scope = this.Services.CreateScope();
		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
		var getResult = await accountRepository.GetByUsernameAsync(username);
		if (!getResult.TryGetValue<Domain.Accounts.Entities.Account>(out var account) || account is null)
		{
			return;
		}

		if (account.HasSubscription())
		{
			var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
			var subscriptionResult = await subscriptionRepository.GetByIdAsync(account.SubscriptionId!.Value, allowDeleted: true);
			if (subscriptionResult.TryGetValue<Domain.Subscriptions.Entities.Subscription>(out var subscription) &&
			    subscription is not null)
			{
				await subscriptionRepository.DeleteAsync(subscription);
			}
		}
		await accountRepository.DeleteAsync(account);
	}

	public async ValueTask EnsureAccountDoesNotExist(AccountCredentials credentials)
	{
		using var scope = this.Services.CreateScope();
		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
		var getResult = await accountRepository.GetByIdAsync(credentials.Id, allowDeleted: true);
		if (!getResult.TryGetValue<Domain.Accounts.Entities.Account>(out var account) || account is null)
		{
			return;
		}

		if (account.HasSubscription())
		{
			var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
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
		using var scope = this.Services.CreateScope();
		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
		var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
		var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

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
		using var scope = this.Services.CreateScope();
		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
		var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
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
