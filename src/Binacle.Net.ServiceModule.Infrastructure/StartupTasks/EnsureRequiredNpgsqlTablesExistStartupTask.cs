using Binacle.Net.Kernel.StartupTasks;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Binacle.Net.ServiceModule.Infrastructure.StartupTasks;

internal class EnsureRequiredNpgsqlTablesExistStartupTask  : IStartupTask
{
	private readonly IServiceProvider serviceProvider;

	public EnsureRequiredNpgsqlTablesExistStartupTask(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using var scope = this.serviceProvider.CreateScope();
		var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();

		var connection = dataSource.CreateConnection();
		await connection!.OpenAsync(cancellationToken);

		await NpgsqlAccountRepository.EnsureTableExistsAsync(connection);
		await NpgsqlSubscriptionRepository.EnsureTableExistsAsync(connection);

	}
}
