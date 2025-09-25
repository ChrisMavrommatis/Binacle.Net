using System.Data;
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
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.ServiceModule.Infrastructure.Providers;

internal class SqliteInfrastructureProvider : IInfrastructureProvider
{
	public string ConnectionStringName => "Sqlite";
	
	public void Register(IHostApplicationBuilder builder, ConnectionString connectionString)
	{
		
		builder.Services.AddTransient<SqliteConnection>(sp => new SqliteConnection(connectionString));
		builder.Services.AddTransient<IDbConnection>(sp => sp.GetRequiredService<SqliteConnection>());
		builder.Services
			.AddScoped<IAccountRepository, SqliteAccountRepository>()
			.AddScoped<ISubscriptionRepository, SqliteSubscriptionRepository>();
		
		builder.Services.AddHealthCheck<SqliteHealthCheck>(
			"Database",
			HealthStatus.Unhealthy,
			new[] { "Service" }
		);
		
		builder.Services.AddStartupTask<EnsureRequiredSqliteTablesExistStartupTask>();
	}
}
