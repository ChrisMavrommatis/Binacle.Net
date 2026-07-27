using System.Net;
using Binacle.Net.Configuration;
// Microsoft.AspNetCore.Builder ships its own ForwardedHeadersExtensions; the alias keeps this file on ours.
using ForwardedHeadersExtensions = Binacle.Net.ExtensionMethods.ForwardedHeadersExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UnitTests;

// ConfigureForwardedHeaders only writes options; whether a caller actually gets resolved is the framework acting
// on them. These map our configuration the way the app does, hand the result to the real framework middleware,
// and read back the address everything downstream would rate limit and allow-list on.
[Trait("Behavioral Tests", "Ensures forwarded headers resolve the caller as configured")]
public class ForwardedHeadersPipelineTests
{
	private const string forwardedFor = "X-Forwarded-For";

	// The address the socket appears to come from - the proxy, in every case here. A DefaultHttpContext has no
	// peer, so it is set by hand, exactly where Kestrel would have set it.
	private const string proxyAddress = "10.0.0.7";

	private static ForwardedHeadersMiddleware MiddlewareWith(
		bool enabled = true,
		bool trustLoopback = false,
		bool trustPrivateNetworks = true,
		string? trustedProxy = null,
		int forwardLimit = 1,
		string? forwardedForHeaderName = null
	)
	{
		var configured = new ForwardedHeadersConfigurationOptions
		{
			Enabled = enabled,
			TrustLoopback = trustLoopback,
			TrustPrivateNetworks = trustPrivateNetworks,
			TrustedProxies = trustedProxy is null ? null : [trustedProxy],
			ForwardLimit = forwardLimit,
			ForwardedForHeaderName = forwardedForHeaderName
		};

		var options = new ForwardedHeadersOptions();
		ForwardedHeadersExtensions.Apply(configured, options);

		return new ForwardedHeadersMiddleware(_ => Task.CompletedTask, NullLoggerFactory.Instance, Options.Create(options));
	}

	private static HttpContext ContextWith(params (string Name, string Value)[] headers)
	{
		var context = new DefaultHttpContext();
		context.Connection.RemoteIpAddress = IPAddress.Parse(proxyAddress);

		foreach (var header in headers)
		{
			context.Request.Headers[header.Name] = header.Value;
		}

		return context;
	}

	[Fact]
	public async Task A_Trusted_Hop_Resolves_The_Caller()
	{
		var middleware = MiddlewareWith(trustPrivateNetworks: true);

		var context = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse("203.0.113.5"));
	}

	// 10.0.0.7 is not named and private networks are not trusted, so the header is somebody's unverified text.
	[Fact]
	public async Task An_Untrusted_Hop_Leaves_The_Socket_Address_In_Place()
	{
		var middleware = MiddlewareWith(trustPrivateNetworks: false, trustedProxy: "192.168.1.1");

		var context = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse(proxyAddress));
	}

	[Fact]
	public async Task A_Named_Trusted_Proxy_Resolves_The_Caller()
	{
		var middleware = MiddlewareWith(trustPrivateNetworks: false, trustedProxy: proxyAddress);

		var context = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse("203.0.113.5"));
	}

	// Two hops with a limit of one: only the nearest is read, so padding the header cannot push the result
	// further back than the real topology.
	[Fact]
	public async Task An_Entry_Beyond_The_Forward_Limit_Is_Ignored()
	{
		var middleware = MiddlewareWith(forwardLimit: 1);

		var context = ContextWith((forwardedFor, "198.51.100.9, 203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse("203.0.113.5"));
	}

	[Fact]
	public async Task A_Higher_Forward_Limit_Reads_Further_Back()
	{
		var middleware = MiddlewareWith(forwardLimit: 2, trustedProxy: "203.0.113.5");

		var context = ContextWith((forwardedFor, "198.51.100.9, 203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse("198.51.100.9"));
	}

	// The case ForwardedForHeaderName exists for: a CDN that overwrites its own single-value header, leaving
	// nothing of what the caller sent in X-Forwarded-For.
	[Theory]
	[InlineData("CF-Connecting-IP")]
	[InlineData("X-Real-IP")]
	[InlineData("X-Azure-ClientIP")]
	public async Task A_Vendor_Header_Name_Resolves_The_Caller(string headerName)
	{
		var middleware = MiddlewareWith(forwardedForHeaderName: headerName);

		var context = ContextWith((headerName, "203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse("203.0.113.5"));
	}

	[Fact]
	public async Task A_Vendor_Header_Name_Leaves_X_Forwarded_For_Unread()
	{
		var middleware = MiddlewareWith(forwardedForHeaderName: "CF-Connecting-IP");

		var context = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse(proxyAddress));
	}

	// Disabled has to stay disabled even with a trusted proxy in front, because the header is the one thing a
	// caller can write freely.
	[Fact]
	public async Task A_Disabled_Configuration_Reads_No_Header()
	{
		var middleware = MiddlewareWith(enabled: false);

		var context = ContextWith((forwardedFor, "203.0.113.5"));
		await middleware.Invoke(context);

		context.Connection.RemoteIpAddress.ShouldBe(IPAddress.Parse(proxyAddress));
	}
}
