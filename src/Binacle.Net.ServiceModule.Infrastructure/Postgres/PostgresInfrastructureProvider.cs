using System.Data;
using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Binacle.Net.ServiceModule.Infrastructure.Postgres;

internal class PostgresInfrastructureProvider : IInfrastructureProvider
{
	public string ConnectionStringName => "Postgres";
	
	public void Register(IHostApplicationBuilder builder, ConnectionString connectionString)
	{
		
		builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));
		builder.Services
			.AddScoped<IAccountRepository, PostgresAccountRepository>()
			.AddScoped<ISubscriptionRepository, PostgresSubscriptionRepository>();
			 
		builder.Services.AddStartupTask<EnsureRequiredPostgresTablesExistStartupTask>();
	}
}
