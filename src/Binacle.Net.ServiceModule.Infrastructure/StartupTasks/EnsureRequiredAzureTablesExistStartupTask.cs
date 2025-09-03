using Azure.Data.Tables;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.Infrastructure.StartupTasks;

internal class EnsureRequiredAzureTablesExistStartupTask : IStartupTask
{
	private readonly IServiceProvider serviceProvider;

	public EnsureRequiredAzureTablesExistStartupTask(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using var scope = this.serviceProvider.CreateScope();
		var tableServiceClient = scope.ServiceProvider.GetRequiredService<TableServiceClient>();
		
		await tableServiceClient.CreateTableIfNotExistsAsync(AzureTablesAccountRepository.TableName, cancellationToken);
		await tableServiceClient.CreateTableIfNotExistsAsync(AzureTablesSubscriptionRepository.TableName, cancellationToken);
	}
}
