using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common.Services;
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
		
		builder.Services
			.AddSingleton<IPasswordService, PasswordService>()
			.AddSingleton<IPasswordHasher, PlainTextPasswordHasher>()
			.AddSingleton<IPasswordHasher, Sha256PasswordHasher>()
			.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>()
			.AddScoped<IAccountRepository, InMemoryAccountRepository>()
			.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>();
		
		// TODO: Actual implementation
		/* var azureStorageConnectionString = builder.Configuration
		 	.GetConnectionStringWithEnvironmentVariableFallback("AzureStorage");
		
		 if (azureStorageConnectionString is not null)
		 {
		 	Log.Information("Registering {StorageProvider} as infrastructure provider", "AzureStorage");
		
		 	builder.Services.AddScoped<IUserRepository, AzureTablesUserRepository>();
		
		 	builder.Services.AddHealthCheck<AzureTablesHeathCheck>(
		 		"AzureTables",
		 		HealthStatus.Unhealthy,
		 		new[] { "Database" }
		 	);
		
		 	
		 	builder.Services.AddAzureClients(clientBuilder =>
		 	{
		 		clientBuilder.AddTableServiceClient(azureStorageConnectionString);
		 	});
		 }
				
		 var userRepositoryServiceDescriptor = builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IUserRepository));
		 if (userRepositoryServiceDescriptor is null)
		 {
		 	var ex = new ApplicationException("IUserRepository was not registered");
		 	Log.Fatal(ex, "No Database provider was registered, please check your configuration");
		 	throw ex;
		 }
		*/
		builder.Services.AddStartupTask<EnsureDefaultAdminAccountExistsStartupTask>();

		return builder;
	}
}

