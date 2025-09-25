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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Binacle.Net.ServiceModule.Infrastructure.Providers;

internal class NpgsqlInfrastructureProvider : IInfrastructureProvider
{
	public string ConnectionStringName => "Postgres";

	public void Register(IHostApplicationBuilder builder, ConnectionString connectionString)
	{
		builder.Services.AddSingleton<NpgsqlDataSource>(sp =>
		{
			var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
			return dataSourceBuilder.Build();
		});

		builder.Services.AddTransient<NpgsqlConnection>(sp => sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());
		builder.Services.AddTransient<IDbConnection>(sp => sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());
		builder.Services
			.AddScoped<IAccountRepository, NpgsqlAccountRepository>()
			.AddScoped<ISubscriptionRepository, NpgsqlSubscriptionRepository>();

		builder.Services.AddHealthCheck<NpgsqlHealthCheck>(
			"Database",
			HealthStatus.Unhealthy,
			new[] { "Service" }
		);

		builder.Services.AddStartupTask<EnsureRequiredNpgsqlTablesExistStartupTask>();
	}
}
