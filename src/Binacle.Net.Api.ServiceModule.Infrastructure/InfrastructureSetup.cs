using Binacle.Net.Api.Kernel;
using Binacle.Net.Api.ServiceModule.Domain.Users.Data;
using Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables;
using Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Data;
using Binacle.Net.Api.ServiceModule.Infrastructure.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Binacle.Net.Api.ServiceModule.Infrastructure;

public static class InfrastructureSetup
{
	public static T AddInfrastructureLayerServices<T>(this T builder)
		where T : IHostApplicationBuilder
	{
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


		builder.Services.AddStartupTask<EnsureAdminUserExistsStartupTask>();

		return builder;
	}
}

