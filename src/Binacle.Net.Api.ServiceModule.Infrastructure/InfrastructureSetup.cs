using Azure.Data.Tables;
using Binacle.Net.Api.Kernel;
using Binacle.Net.Api.ServiceModule.Domain.Users.Data;
using Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables;
using Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Data;
using Binacle.Net.Api.ServiceModule.Infrastructure.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace Binacle.Net.Api.ServiceModule.Infrastructure;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, IConfiguration configuration)
	{
		var azureStorageConnectionString = configuration.GetConnectionStringWithEnvironmentVariableFallback(
				"AzureStorage",
				"AZURESTORAGE_CONNECTION_STRING"
		);


		if (azureStorageConnectionString is not null)
		{
			Log.Information("Registering {StorageProvider} as infrastructure provider", "AzureStorage");

			services.AddScoped<IUserRepository, AzureTablesUserRepository>();

			services.Configure<HealthCheckServiceOptions>(options =>
			{
				options.Registrations.Add(new HealthCheckRegistration(
					"AzureTables",
					sp => new AzureTablesHeathCheck(sp.GetRequiredService<TableServiceClient>()),
					failureStatus: HealthStatus.Unhealthy,
					["Database"]
				));
			});

			// Register Azure
			services.AddAzureClients(clientBuilder =>
			{
				clientBuilder.AddTableServiceClient(azureStorageConnectionString);
			});
		}


		services.AddStartupTask<EnsureAdminUserExistsStartupTask>();

		return services;
	}
}

