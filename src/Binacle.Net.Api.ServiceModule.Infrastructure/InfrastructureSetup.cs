﻿using Binacle.Net.Api.Kernel.Helpers;
using Binacle.Net.Api.ServiceModule.Domain.Users.Data;
using Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables.Users.Data;
using Binacle.Net.Api.ServiceModule.Infrastructure.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Binacle.Net.Api.ServiceModule.Infrastructure;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, IConfiguration configuration)
	{
		var azureStorageConnectionString = SetupConfigurationHelper.GetConnectionStringWithEnvironmentVariableFallback(
				configuration,
				"AzureStorage",
				"AZURESTORAGE_CONNECTION_STRING");


		if (!string.IsNullOrWhiteSpace(azureStorageConnectionString))
		{
			Log.Information("Registering {StorageProvider} as infrastructure provider", "AzureStorage");

			services.AddScoped<IUserRepository, AzureTablesUserRepository>();

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

