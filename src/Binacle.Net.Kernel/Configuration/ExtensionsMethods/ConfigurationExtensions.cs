using Binacle.Net.Kernel;
using Binacle.Net.Kernel.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Binacle.Net;

public static class ConfigurationExtensions
{
	public static ConnectionString? GetConnectionStringWithEnvironmentVariableFallback(
		this IConfiguration configuration,
		string name
		)
	{
		var connectionString = configuration?.GetConnectionString(name);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, "Configuration File");
			return new ConnectionString(connectionString);
		}

		var variable = $"{name.ToUpperInvariant()}_CONNECTION_STRING";

		connectionString = Environment.GetEnvironmentVariable(variable);

		if (!string.IsNullOrWhiteSpace(connectionString))
		{
			Log.Information("Connection String {connectionString} found in {location}", name, $"Environment Variable: {variable}");
			return new ConnectionString(connectionString);
		}

		Log.Warning("Connection String {connectionString} not found", name);
		return null;
	}

	public static void AddJsonConfiguration<TBuilder>(
		this TBuilder builder, 
		string filePath, 
		string? environmentFilePath = null,
		bool optional = false,
		bool reloadOnChange = false
		)
		where TBuilder : IHostApplicationBuilder
	{
		builder.Configuration
			.AddJsonFile(filePath, optional: optional, reloadOnChange: reloadOnChange);

		if (!string.IsNullOrWhiteSpace(environmentFilePath))
		{
			builder.Configuration
				.AddJsonFile(environmentFilePath, optional: true, reloadOnChange: reloadOnChange);
		}
		
		builder.Configuration
			.AddEnvironmentVariables();
	}
	
	public static void AddValidatableJsonConfigurationOptions<TOptions>(
		this IHostApplicationBuilder builder,
		Action<TOptions>? postConfigure = null
		)
		where TOptions : class, IConfigurationOptions
	{
		builder.Configuration
			.AddJsonFile(TOptions.FilePath, optional: TOptions.Optional, reloadOnChange: TOptions.ReloadOnChange);

		var environmentFilePath = TOptions.GetEnvironmentFilePath(builder.Environment.EnvironmentName);
		if (!string.IsNullOrWhiteSpace(environmentFilePath))
		{
			builder.Configuration
				.AddJsonFile(environmentFilePath, optional: true, reloadOnChange: TOptions.ReloadOnChange);
		}
			
		builder.Configuration
			.AddEnvironmentVariables();
		
		var optionsBuilder = builder.Services
			.AddOptions<TOptions>()
			.Bind(builder.Configuration.GetSection(TOptions.SectionName))
			.ValidateFluently()
			.ValidateOnStart();
		
		if(postConfigure is not null)
		{
			optionsBuilder.PostConfigure(postConfigure);
		}
	}
	
	public static TOptions? GetConfigurationOptions<TOptions>(
		this IConfigurationManager configuration
		)
		where TOptions : class, IConfigurationOptions
	{
		return configuration.GetSection(TOptions.SectionName)
			.Get<TOptions>();
	}
}
