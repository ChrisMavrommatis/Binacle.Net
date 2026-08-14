using Binacle.Net.DiagnosticsModule.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// The two states this warns about are the ones an operator cannot see from outside: the app is reading the
// proxy as the caller, and everything downstream agrees. Both are silent otherwise, so what it logs is the
// behaviour under test.
[Trait("Behavioral Tests", "Ensures an ignored forwarded header is reported once")]
public class ForwardedHeadersDiagnosticsMiddlewareTests
{
	private const string forwardedFor = "X-Forwarded-For";
	private const string originalFor = "X-Original-For";

	private static (ForwardedHeadersDiagnosticsMiddleware Middleware, CapturingLogger<ForwardedHeadersDiagnosticsMiddleware> Logger)
		MiddlewareWith(
			bool forwardedHeadersEnabled, 
			string? forwardedForHeaderName = null
		)
	{
		var logger = new CapturingLogger<ForwardedHeadersDiagnosticsMiddleware>();
		var options = Options.Create(new ForwardedHeadersOptions
		{
			// None is what ConfigureForwardedHeaders writes when the feature is off.
			ForwardedHeaders = forwardedHeadersEnabled
				? ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
				: ForwardedHeaders.None
		});

		if (forwardedForHeaderName is not null)
		{
			options.Value.ForwardedForHeaderName = forwardedForHeaderName;
		}

		return (new ForwardedHeadersDiagnosticsMiddleware(_ => Task.CompletedTask, logger, options), logger);
	}

	private static HttpContext ContextWith(params (string Name, string Value)[] headers)
	{
		var context = new DefaultHttpContext();
		context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.7");

		foreach (var header in headers)
		{
			context.Request.Headers[header.Name] = header.Value;
		}

		return context;
	}

	[Fact]
	public async Task A_Forwarded_Header_With_The_Feature_Off_Warns()
	{
		var (middleware, logger) = MiddlewareWith(forwardedHeadersEnabled: false);

		var context = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.InvokeAsync(context);

		logger.Warnings.ShouldHaveSingleItem().ShouldContain("Forwarded headers are disabled");
	}

	// The trust list did not name the proxy, so the framework dropped the header and wrote no original-for.
	[Fact]
	public async Task A_Forwarded_Header_The_Framework_Did_Not_Apply_Warns()
	{
		var (middleware, logger) = MiddlewareWith(forwardedHeadersEnabled: true);

		var cotnext = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.InvokeAsync(cotnext);

		logger.Warnings.ShouldHaveSingleItem().ShouldContain("not in the trust list");
	}

	// Original-for is written only when an address was actually replaced, which is the working case.
	[Fact]
	public async Task An_Applied_Forwarded_Header_Says_Nothing()
	{
		var (middleware, logger) = MiddlewareWith(forwardedHeadersEnabled: true);

		var context = ContextWith((forwardedFor, "203.0.113.5"), (originalFor, "10.0.0.7"));
		await middleware.InvokeAsync(context);

		logger.Warnings.ShouldBeEmpty();
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task A_Request_Without_A_Forwarded_Header_Says_Nothing(bool forwardedHeadersEnabled)
	{
		var (middleware, logger) = MiddlewareWith(forwardedHeadersEnabled);

		await middleware.InvokeAsync(ContextWith());

		logger.Warnings.ShouldBeEmpty();
	}

	// A misconfigured proxy sends the header on every request, and warning each time buries the log.
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task Repeated_Requests_Warn_Once(bool forwardedHeadersEnabled)
	{
		var (middleware, logger) = MiddlewareWith(forwardedHeadersEnabled);

		for (var request = 0; request < 5; request++)
		{
			var context = ContextWith((forwardedFor, "203.0.113.5"));
			await middleware.InvokeAsync(context);
		}

		logger.Warnings.Count.ShouldBe(1);
	}

	// The diagnostic has to follow the configured header name rather than assume X-Forwarded-For.
	[Fact]
	public async Task A_Vendor_Header_Name_Is_Watched_Instead()
	{
		var (middleware, logger) = MiddlewareWith(forwardedHeadersEnabled: true, forwardedForHeaderName: "CF-Connecting-IP");

		await middleware.InvokeAsync(
			ContextWith((forwardedFor, "203.0.113.5"))
		);
		logger.Warnings.ShouldBeEmpty();

		await middleware.InvokeAsync(
			ContextWith(("CF-Connecting-IP", "203.0.113.5"))
		);
		logger.Warnings.ShouldHaveSingleItem().ShouldContain("CF-Connecting-IP");
	}

	[Fact]
	public async Task The_Request_Always_Continues()
	{
		var logger = new CapturingLogger<ForwardedHeadersDiagnosticsMiddleware>();
		var reachedNext = false;
		var middleware = new ForwardedHeadersDiagnosticsMiddleware(
			_ =>
			{
				reachedNext = true;
				return Task.CompletedTask;
			},
			logger,
			Options.Create(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.None })
		);

		var context = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.InvokeAsync(context);

		reachedNext.ShouldBeTrue();
	}
}
