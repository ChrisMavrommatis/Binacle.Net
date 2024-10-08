﻿using Azure.Data.Tables;
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
		try
		{
			var response = await tableServiceClient.GetPropertiesAsync(cancellationToken: cancellationToken);
			return HealthCheckResult.Healthy();
		}
		catch (Exception ex)
		{
			return new HealthCheckResult(context.Registration.FailureStatus, "Health check for Azure Tables failed", ex);
		}
	}
}
