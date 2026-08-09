using System.Data;
using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common;
using Binacle.Net.ServiceModule.Infrastructure.HealthChecks;
using Binacle.Net.ServiceModule.Infrastructure.StartupTasks;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.ServiceModule.Infrastructure.Providers;

internal class SqliteInfrastructureProvider : IInfrastructureProvider
{
	private static readonly string[] ServiceTags = ["Service"];

	public string ConnectionStringName => "Sqlite";
	
	public void Register(IHostApplicationBuilder builder, ConnectionString connectionString)
	{

		builder.Services.AddTransient<SqliteConnection>(sp => CreateConnection(connectionString));
		builder.Services.AddTransient<IDbConnection>(sp => sp.GetRequiredService<SqliteConnection>());
		builder.Services
			.AddScoped<IAccountRepository, SqliteAccountRepository>()
			.AddScoped<ISubscriptionRepository, SqliteSubscriptionRepository>();
		
		builder.Services.AddHealthCheck<SqliteHealthCheck>(
			"Database",
			HealthStatus.Unhealthy,
			ServiceTags
		);
		
		builder.Services.AddStartupTask<EnsureRequiredSqliteTablesExistStartupTask>();
	}

	internal static SqliteConnection CreateConnection(string connectionString)
	{
		var connection = new SqliteConnection(connectionString);
		connection.Open();

		// SQLite has one file-level write lock and no async I/O, so concurrent writers collide. WAL lets reads
		// and writes run together; busy_timeout waits on a contended lock instead of throwing "database is
		// locked". Neither is a connection-string keyword, so they are set by PRAGMA on open.
		using var command = connection.CreateCommand();
		command.CommandText = "PRAGMA journal_mode=WAL; PRAGMA busy_timeout=5000;";
		command.ExecuteNonQuery();

		return connection;
	}
}
