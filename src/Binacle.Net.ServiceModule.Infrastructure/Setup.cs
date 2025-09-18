using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Providers;
using Binacle.Net.ServiceModule.Infrastructure.Common;
using Binacle.Net.ServiceModule.Infrastructure.Common.Services;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Binacle.Net.ServiceModule.Infrastructure;

public static class Setup
{
	private static readonly IInfrastructureProvider[] _infrastructureProviders =
	[
		new AzureStorageInfrastructureProvider(),
		new NpgsqlInfrastructureProvider(),
		new SqliteInfrastructureProvider()
	];

	public static T AddInfrastructure<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		builder.Services
			.AddSingleton<IPasswordService, PasswordService>()
			.AddSingleton<IPasswordHasher, Services.PlainTextPasswordHasher>()
			.AddSingleton<IPasswordHasher, Services.Sha256PasswordHasher>()
			.AddSingleton<IPasswordHasher, Services.Pbkdf2PasswordHasher>();

		foreach (var infrastructureProvider in _infrastructureProviders)
		{
			var connectionString = builder.Configuration
				.GetConnectionStringWithEnvironmentVariableFallback(infrastructureProvider.ConnectionStringName);

			if (connectionString is not null)
			{
				Log.Information("Registering {StorageProvider} as infrastructure provider",
					infrastructureProvider.ConnectionStringName);
				infrastructureProvider.Register(builder, connectionString);
				break;
			}
		}

		builder.Services
			.AssertServiceWasRegistered<IAccountRepository>()
			.AssertServiceWasRegistered<ISubscriptionRepository>();

		builder.Services.AddStartupTask<EnsureDefaultAdminAccountExistsStartupTask>();
		
		var backupToS3ConnectionString = builder.Configuration
			.GetConnectionStringWithEnvironmentVariableFallback("BackupToS3");
		if (backupToS3ConnectionString is not null)
		{
			Log.Information("Registering {ServiceName} as a hosted service", "BackupToS3");
			builder.Services.AddHostedService<Services.BackupToS3>(sp =>
			{
				return new Services.BackupToS3(
					sp.GetRequiredService<IHostEnvironment>(),
					sp.GetRequiredService<TimeProvider>(),
					sp.GetRequiredService<ILogger<Services.BackupToS3>>(),
					backupToS3ConnectionString
				);
			});
		}
		
		

		return builder;
	}

	private static IServiceCollection AssertServiceWasRegistered<T>(this IServiceCollection services)
	{
		var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(T));
		if (serviceDescriptor is null)
		{
			var ex = new ApplicationException($"{typeof(T).Name} was not registered");
			Log.Fatal(ex, "No Database provider was registered for {Service}, please check your configuration",
				typeof(T).Name);
			throw ex;
		}

		return services;
	}
}
