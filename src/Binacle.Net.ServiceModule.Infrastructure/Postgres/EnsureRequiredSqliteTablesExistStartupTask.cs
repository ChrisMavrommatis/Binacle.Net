using System.Data;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Binacle.Net.ServiceModule.Infrastructure.Postgres;

internal class EnsureRequiredPostgresTablesExistStartupTask  : IStartupTask
{
	private readonly IServiceProvider serviceProvider;

	public EnsureRequiredPostgresTablesExistStartupTask(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using var scope = this.serviceProvider.CreateScope();
		var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>() as NpgsqlConnection;

		await connection!.OpenAsync(cancellationToken);

		await PostgresAccountRepository.EnsureTableExistsAsync(connection);
		await PostgresSubscriptionRepository.EnsureTableExistsAsync(connection);

	}

	
}
