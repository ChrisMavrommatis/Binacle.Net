using Binacle.Net.Kernel;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.AzureTables;
using Binacle.Net.ServiceModule.Infrastructure.Common.Services;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

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
			.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
		
		var azureStorageConnectionString = builder.Configuration
		 	.GetConnectionStringWithEnvironmentVariableFallback("AzureStorage");
		
		 if (azureStorageConnectionString is not null)
		 {
			 Log.Information("Registering {StorageProvider} as infrastructure provider", "AzureStorage");
			 builder.Services
				 .AddScoped<IAccountRepository, AzureTablesAccountRepository>()
				 .AddScoped<ISubscriptionRepository, AzureTablesSubscriptionRepository>();
		
			 builder.Services.AddHealthCheck<AzureTablesHeathCheck>(
				 "AzureTables",
				 HealthStatus.Unhealthy,
				 new[] { "Database" }
			 );
		 	
			 builder.Services.AddAzureClients(clientBuilder =>
			 {
				 clientBuilder.AddTableServiceClient(azureStorageConnectionString);
			 });

			 builder.Services.AddStartupTask<EnsureRequiredAzureTablesExistStartupTask>();
		 }
		 
		 builder.Services
			 .AssertServiceWasRegistered<IAccountRepository>()
			 .AssertServiceWasRegistered<ISubscriptionRepository>();
		
		builder.Services.AddStartupTask<EnsureDefaultAdminAccountExistsStartupTask>();

		return builder;
	}

	private static IServiceCollection AssertServiceWasRegistered<T>(this IServiceCollection services)
	{
		var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(T));
		if (serviceDescriptor is null)
		{
			var ex = new ApplicationException($"{typeof(T).Name} was not registered");
			Log.Fatal(ex, "No Database provider was registered for {Service}, please check your configuration", typeof(T).Name);
			throw ex;
		}

		return services;
	}
}

