using Binacle.Net.Api.Kernel.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Binacle.Net.Api.Kernel;

public static class ConfigurationExtensions
{
	public static ConnectionString? GetConnectionStringWithEnvironmentVariableFallback(
		this IConfiguration configuration,
		string name,
		string variable
		)
	{
		var connectionString = configuration?.GetConnectionString(name);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, "Configuration File");
			return new ConnectionString(connectionString);
		}

		connectionString = Environment.GetEnvironmentVariable(variable);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, $"Environment Variable: {variable}");
			return new ConnectionString(connectionString);
		}

		Log.Warning("Connection String {connectionString} not found", name);
		return null;
	}
}
