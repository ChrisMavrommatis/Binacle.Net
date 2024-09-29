using Azure.Data.Tables;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Binacle.Net.Api.ServiceModule.Infrastructure.AzureTables;

public class AzureTablesHeathCheck : IHealthCheck
{
	private readonly TableServiceClient tableServiceClient;

	public AzureTablesHeathCheck(TableServiceClient tableServiceClient)
	{
		this.tableServiceClient = tableServiceClient;
	}

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		// azure table health 
		return HealthCheckResult.Healthy();
	}
}
