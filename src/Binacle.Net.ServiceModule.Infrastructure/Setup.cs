using Binacle.Net.Kernel;
using Binacle.Net.ServiceModule.Application.Authentication.Services;
using Binacle.Net.ServiceModule.Domain.Users.Data;
using Binacle.Net.ServiceModule.Infrastructure.Authentication.Services;
using Binacle.Net.ServiceModule.Infrastructure.AzureTables;
using Binacle.Net.ServiceModule.Infrastructure.AzureTables.Users.Data;
using Binacle.Net.ServiceModule.Infrastructure.Services;
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
		
		// Register Services
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
		
		var azureStorageConnectionString = builder.Configuration
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

			// Register Azure
			builder.Services.AddAzureClients(clientBuilder =>
			{
				clientBuilder.AddTableServiceClient(azureStorageConnectionString);
			});
		}
		
		// see if IUserRepository was registered
		var userRepositoryServiceDescriptor = builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IUserRepository));
		if (userRepositoryServiceDescriptor is null)
		{
			var ex = new ApplicationException("IUserRepository was not registered");
			Log.Fatal(ex, "No Database provider was registered, please check your configuration");
			throw ex;
		}

		builder.Services.AddStartupTask<EnsureDefaultAdminAccountExistsStartupTask>();

		return builder;
	}
}

