using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.Middleware;

// Says out loud that a forwarding header arrived and did not take effect. Two ways that happens, and both are
// silent otherwise: the feature is off, or the trust list does not name the proxy in front - which the framework
// reports only as "Unknown proxy" at Debug level, which nobody runs in production. Either way every component
// downstream reads the proxy as the caller: the health check allow-list, the login throttle, the logs.
//
// Nothing here trusts a header. They are read only to decide whether to warn.
internal class ForwardedHeadersDiagnosticsMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<ForwardedHeadersDiagnosticsMiddleware> logger;
	private readonly ForwardedHeadersOptions options;

	// One warning for the life of the process, not one per request. Whichever of the two states this is, it is
	// fixed at startup: a misconfigured proxy sends the header on every call, and a warning per request buries
	// the log it is trying to draw attention to.
	private int warned;

	public ForwardedHeadersDiagnosticsMiddleware(
		RequestDelegate next,
		ILogger<ForwardedHeadersDiagnosticsMiddleware> logger,
		IOptions<ForwardedHeadersOptions> forwardedHeadersOptions
	)
	{
		this.next = next;
		this.logger = logger;
		this.options = forwardedHeadersOptions.Value;
	}

	// ConfigureForwardedHeaders writes None when the feature is off, deliberately, so that
	// ASPNETCORE_FORWARDEDHEADERS_ENABLED cannot switch the middleware on behind us. That makes None the app's
	// own answer to whether the feature is live.
	private bool ForwardedHeadersEnabled => this.options.ForwardedHeaders != ForwardedHeaders.None;

	public async Task InvokeAsync(HttpContext context)
	{
		this.WarnOnceIfTheHeaderWasIgnored(context);

		await this.next(context);
	}

	private void WarnOnceIfTheHeaderWasIgnored(HttpContext context)
	{
		// Said it already, so every later request leaves without touching the headers. Volatile because other
		// requests write this field: a plain read lets the JIT keep a cached copy instead of re-checking memory.
		if (Volatile.Read(ref this.warned) == 1)
		{
			return;
		}

		if (!this.HeaderArrivedAndWasNotApplied(context.Request))
		{
			return;
		}

		// Two requests can arrive at once and both get this far. Exchange sets the flag and hands back what was
		// there before, so exactly one of them is told it was first, and only that one logs.
		var someoneElseWarnedFirst = Interlocked.Exchange(ref this.warned, 1) == 1;

		if (someoneElseWarnedFirst)
		{
			return;
		}

		this.Warn(context);
	}

	private bool HeaderArrivedAndWasNotApplied(HttpRequest request)
	{
		if (!request.Headers.ContainsKey(this.options.ForwardedForHeaderName))
		{
			return false;
		}

		// With the feature off nothing was ever going to be applied. With it on, the original-for header is the
		// signal rather than the forwarded-for one: the framework rewrites the forwarded header as it consumes
		// entries and removes it once empty, so what is left says nothing about whether an address was replaced.
		// Original-for is written only when one was.
		return !this.ForwardedHeadersEnabled || !request.Headers.ContainsKey(this.options.OriginalForHeaderName);
	}

	private void Warn(HttpContext context)
	{
		if (!this.ForwardedHeadersEnabled)
		{
			this.logger.LogWarning(
				"Forwarded headers are disabled, but a request arrived carrying {Header}. The caller is being read "
				+ "as {RemoteIp}, which is the proxy. Enable forwarded headers in ForwardedHeaders.json and trust "
				+ "that proxy. Logged once.",
				this.options.ForwardedForHeaderName,
				context.Connection.RemoteIpAddress
			);
			return;
		}

		this.logger.LogWarning(
			"A request arrived carrying {Header}, but it was not applied: {RemoteIp} is not in the trust list, so "
			+ "the caller is being read as the proxy. Add it to TrustedProxies in ForwardedHeaders.json, or turn on "
			+ "TrustPrivateNetworks if the proxy sits on a private network. Logged once.",
			this.options.ForwardedForHeaderName,
			context.Connection.RemoteIpAddress
		);
	}
}
