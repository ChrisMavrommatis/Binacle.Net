using System.Net;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Middleware;
using Binacle.Net.DiagnosticsModule.UnitTests.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// The middleware reads only the connection's remote address, so a DefaultHttpContext drives it fully — no test
// host, no server. Who gets a 403 is what an operator can lock themselves out with.
[Trait("Behavioral Tests", "Ensures health check IP protection behaves as expected")]
public class HealthChecksProtectionMiddlewareTests
{
	private const string healthPath = "/_health";

	private static HttpContext ContextFor(string path, string? remoteIpAddress)
	{
		var context = new DefaultHttpContext();
		context.Request.Path = path;
		context.Connection.RemoteIpAddress = remoteIpAddress is null ? null : IPAddress.Parse(remoteIpAddress);
		return context;
	}

	private static (HealthChecksProtectionMiddleware Middleware, Func<bool> ReachedHealthCheck) MiddlewareWith(
		params string[]? restrictedIPs
	)
	{
		var reachedHealthCheck = false;
		var options = Options.Create(new HealthCheckConfigurationOptions
		{
			Enabled = true,
			Path = healthPath,
			RestrictedIPs = restrictedIPs
		});

		var middleware = new HealthChecksProtectionMiddleware(
			_ =>
			{
				reachedHealthCheck = true;
				return Task.CompletedTask;
			},
			NullLogger<HealthChecksProtectionMiddleware>.Instance,
			options
		);

		return (middleware, () => reachedHealthCheck);
	}

	[Theory]
	[ClassData(typeof(HealthCheckCallerProvider))]
	public async Task A_Caller_Reaches_The_Health_Check_Only_When_The_Allow_List_Covers_It(
		string entry,
		string callerAddress,
		bool expectedAllowed
	)
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith(entry);
		var context = ContextFor(healthPath, callerAddress);

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBe(expectedAllowed);
		context.Response.StatusCode.ShouldBe(
			expectedAllowed ? StatusCodes.Status200OK : StatusCodes.Status403Forbidden
		);
	}

	[Fact]
	public async Task An_Unknown_Remote_Address_Is_Refused()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith("192.168.1.0/24");
		var context = ContextFor(healthPath, null);

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeFalse();
		context.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
	}

	[Fact]
	public async Task An_Empty_Allow_List_Restricts_Nobody()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith();
		var context = ContextFor(healthPath, "10.0.0.1");

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeTrue();
	}

	[Fact]
	public async Task Requests_Off_The_Health_Path_Are_Never_Restricted()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith("192.168.1.0/24");
		var context = ContextFor("/api/v4/pack/bin", "10.0.0.1");

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeTrue();
	}

	[Fact]
	public void A_Malformed_Entry_That_Slipped_Past_Validation_Fails_On_Construction()
	{
		Should.Throw<InvalidOperationException>(() => MiddlewareWith("192.168.1.0/24", "not-an-address"));
	}
}
