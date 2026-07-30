using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Binacle.Net.ServiceModule.Infrastructure.HealthChecks;

internal class SqliteHealthCheck : IHealthCheck
{
	private readonly SqliteConnection connection;

	public SqliteHealthCheck(SqliteConnection connection)
	{
		this.connection = connection;
	}
	
	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await this.connection!.OpenAsync(cancellationToken).ConfigureAwait(false);
			var command = connection.CreateCommand();
			command.CommandText = "select name from sqlite_master where type='table'";
			var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
			
			await connection.CloseAsync();
			return HealthCheckResult.Healthy("Sqlite");
		}
		catch (Exception ex)
		{
			return new HealthCheckResult(context.Registration.FailureStatus, "Health check for Sqlite failed", ex);
		}
	}
}
