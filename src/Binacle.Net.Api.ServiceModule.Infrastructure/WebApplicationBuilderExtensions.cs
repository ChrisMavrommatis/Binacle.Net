using Binacle.Net.Api.Kernel.Helpers;
using Binacle.Net.Api.ServiceModule.Domain.Users.Data;
using Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Binacle.Net.Api.ServiceModule.Infrastructure;

public static class WebApplicationBuilderExtensions
{
	public static void AddInfrastructureServices(this WebApplicationBuilder builder)
	{
		var azureStorageConnectionString = SetupConfigurationHelper.GetConnectionStringWithEnvironmentVariableFallback(
				builder.Configuration,
				"AzureStorage",
				"AZURESTORAGE_CONNECTION_STRING");


		if (!string.IsNullOrWhiteSpace(azureStorageConnectionString))
		{
			Log.Information("Registering {StorageProvider} as infrastructure provider", "AzureStorage");

			builder.Services.AddScoped<IUserRepository, AzureTablesUserRepository>();

			// Register Azure
			builder.Services.AddAzureClients(clientBuilder =>
			{
				clientBuilder.AddTableServiceClient(azureStorageConnectionString);
			});
		}
	
	}
}
