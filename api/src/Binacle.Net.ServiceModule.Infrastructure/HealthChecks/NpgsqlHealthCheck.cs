using System.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Binacle.Net.ServiceModule.Infrastructure.HealthChecks;

internal class NpgsqlHealthCheck : IHealthCheck
{
	private readonly NpgsqlDataSource dataSource;

	public NpgsqlHealthCheck(NpgsqlDataSource dataSource)
	{
		this.dataSource = dataSource;
	}

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await using var connection = this.dataSource.CreateConnection();
			await connection.OpenAsync(cancellationToken);
			var command = connection.CreateCommand();
			command.CommandText = "SELECT 1";
			var result = await command.ExecuteScalarAsync(cancellationToken);
			return HealthCheckResult.Healthy("Postgres");
			
		}
		catch (Exception ex)
		{
			return new HealthCheckResult(context.Registration.FailureStatus, "Health check for Npgsql failed", ex);
		}
	}
}
