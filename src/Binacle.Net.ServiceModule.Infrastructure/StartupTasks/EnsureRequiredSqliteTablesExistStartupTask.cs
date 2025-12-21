using Binacle.Net.Kernel.StartupTasks;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.Infrastructure.StartupTasks;

internal class EnsureRequiredSqliteTablesExistStartupTask  : IStartupTask
{
	private readonly IServiceProvider serviceProvider;

	public EnsureRequiredSqliteTablesExistStartupTask(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using var scope = this.serviceProvider.CreateScope();
		var connection = scope.ServiceProvider.GetRequiredService<SqliteConnection>();

		await connection!.OpenAsync(cancellationToken);

		await SqliteAccountRepository.EnsureTableExistsAsync(connection);
		await SqliteSubscriptionRepository.EnsureTableExistsAsync(connection);

	}
}
