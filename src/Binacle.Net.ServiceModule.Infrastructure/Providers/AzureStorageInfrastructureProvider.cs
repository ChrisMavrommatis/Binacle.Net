using Binacle.Net.Kernel;
using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common;
using Binacle.Net.ServiceModule.Infrastructure.HealthChecks;
using Binacle.Net.ServiceModule.Infrastructure.StartupTasks;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.ServiceModule.Infrastructure.Providers;

internal class AzureStorageInfrastructureProvider : IInfrastructureProvider
{
	public string ConnectionStringName => "AzureStorage";
	
	public void Register(IHostApplicationBuilder builder, ConnectionString connectionString)
	{
		builder.Services
			.AddScoped<IAccountRepository, AzureTablesAccountRepository>()
			.AddScoped<ISubscriptionRepository, AzureTablesSubscriptionRepository>();
		
		builder.Services.AddHealthCheck<AzureTablesHeathCheck>(
			"Database",
			HealthStatus.Unhealthy,
			new[] { "Service" }
		);
		 	
		builder.Services.AddAzureClients(clientBuilder =>
		{
			clientBuilder.AddTableServiceClient(connectionString);
		});

		builder.Services.AddStartupTask<EnsureRequiredAzureTablesExistStartupTask>();
	}
}
