using Binacle.Net.ServiceModule.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

// The module on, with a live limiter. One per test, never a fixture: the anonymous partition key is a constant,
// so every anonymous caller in a host shares one bucket, and a bucket does not refill inside a run.
public sealed class RateLimitedBinacleApi : WebApplicationFactory<IApiMarker>
{
	// Has to clear the number of rate limited endpoints, so one request to each still sits below the limit.
	public const int ApiUsagePermitLimit = 100;
	public const int AuthTokenPermitLimit = 3;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var preBuildConfigurationValues = new Dictionary<string, string?>
		{
			{ "Features:SERVICE_MODULE", bool.TrueString },
			{ "RateLimiter:ApiUsageAnonymous", $"FixedWindow::{ApiUsagePermitLimit}/60" },
			{ "RateLimiter:AuthToken", $"FixedWindow::{AuthTokenPermitLimit}/60" },
			// Pinned to its own SQLite file on every leg. The shared harness owns binacle-net-service.test.db,
			// and on Postgres or Azure both would land on one database with two default-admin startup tasks
			// racing.
			{ "ConnectionStrings:Sqlite", "DataSource=binacle-net-service.ratelimiting.test.db;" },
			{ "JwtAuth:Issuer", "ForTestsOnly" },
			{ "JwtAuth:Audience", "ForTestsOnly" },
			{ "JwtAuth:TokenSecret", "SecretKeyForTestsOnly_paddedTo70Plus_paddedTo70Plus_paddedTo70Plus_paddedTo70Plus" },
			{ "JwtAuth:ExpirationInSeconds", "3600" }
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(preBuildConfigurationValues)
			.Build();

		builder
			.UseEnvironment("Test")
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				// Overrides the module's own RateLimiter.json, which is added after this runs.
				configurationBuilder.AddInMemoryCollection(preBuildConfigurationValues);
			});

		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
			services.Configure<ServiceModuleOptions>(options =>
			{
				options.DefaultAdminAccount = "testadmin@test.binacle.net:B1n4cl3Adm!nT3st";
			});
		});
	}
}
