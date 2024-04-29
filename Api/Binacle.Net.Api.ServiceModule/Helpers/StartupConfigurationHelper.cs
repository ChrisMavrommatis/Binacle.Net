using Microsoft.Extensions.Configuration;
using Serilog;

namespace Binacle.Net.Api.ServiceModule.Helpers;

internal static class StartupConfigurationHelper
{
	internal static string? GetConnectionStringWithEnvironmentVariableFallback(
		IConfiguration configuration,
		string name,
		string variable
		)
	{
		var connectionString = configuration.GetConnectionString(name);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, "Configuration File");
			return connectionString;
		}

		connectionString = Environment.GetEnvironmentVariable(variable);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, $"Environment Variable: {variable}");
			return connectionString;
		}

		Log.Warning("Connection String {connectionString} not found", name);
		return null;
	}
}
