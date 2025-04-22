using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Application.Authentication.Services;
using Binacle.Net.ServiceModule.Application.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Authentication.Services;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.ServiceModule.Infrastructure;

public static class Setup
{
	public static T AddInfrastructure<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		
		// Register Services
		builder.Services.AddTransient<ITokenService, TokenService>();
		builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();

		builder.Services
			.AddTransient<IAccountRepository, InMemoryAccountRepository>()
			.AddTransient<ISubscriptionRepository, InMemorySubscriptionRepository>();
		// var azureStorageConnectionString = builder.Configuration
		// 	.GetConnectionStringWithEnvironmentVariableFallback("AzureStorage");
		//
		// if (azureStorageConnectionString is not null)
		// {
		// 	Log.Information("Registering {StorageProvider} as infrastructure provider", "AzureStorage");
		//
		// 	builder.Services.AddScoped<IUserRepository, AzureTablesUserRepository>();
		//
		// 	builder.Services.AddHealthCheck<AzureTablesHeathCheck>(
		// 		"AzureTables",
		// 		HealthStatus.Unhealthy,
		// 		new[] { "Database" }
		// 	);
		//
		// 	// Register Azure
		// 	builder.Services.AddAzureClients(clientBuilder =>
		// 	{
		// 		clientBuilder.AddTableServiceClient(azureStorageConnectionString);
		// 	});
		// }
		//
		// // see if IUserRepository was registered
		// var userRepositoryServiceDescriptor = builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IUserRepository));
		// if (userRepositoryServiceDescriptor is null)
		// {
		// 	var ex = new ApplicationException("IUserRepository was not registered");
		// 	Log.Fatal(ex, "No Database provider was registered, please check your configuration");
		// 	throw ex;
		// }

		builder.Services.AddStartupTask<EnsureDefaultAdminAccountExistsStartupTask>();

		return builder;
	}
}

