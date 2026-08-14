using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.Middleware;

// Says out loud that a forwarding header arrived and did not take effect. Two ways that happens, both silent
// otherwise: the feature is off, or the trust list does not name the proxy in front - which the framework
// reports only as "Unknown proxy" at Debug level. Either way every component downstream reads the proxy as the
// caller.
//
// Nothing here trusts a header. They are read only to decide whether to warn.
internal class ForwardedHeadersDiagnosticsMiddleware
{
	private readonly RequestDelegate next;
	private readonly ILogger<ForwardedHeadersDiagnosticsMiddleware> logger;
	private readonly ForwardedHeadersOptions options;

	// One warning for the life of the process. Either state is fixed at startup, and a misconfigured proxy sends
	// the header on every call, so a warning per request would bury the log it is drawing attention to.
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

	// ConfigureForwardedHeaders writes None when the feature is off, so ASPNETCORE_FORWARDEDHEADERS_ENABLED
	// cannot switch the middleware on behind us. None is the app's own answer to whether the feature is live.
	private bool ForwardedHeadersEnabled => this.options.ForwardedHeaders != ForwardedHeaders.None;

	public async Task InvokeAsync(HttpContext context)
	{
		this.WarnOnceIfTheHeaderWasIgnored(context);

		await this.next(context);
	}

	private void WarnOnceIfTheHeaderWasIgnored(HttpContext context)
	{
		// Volatile because other requests write this field: a plain read lets the JIT keep a cached copy.
		if (Volatile.Read(ref this.warned) == 1)
		{
			return;
		}

		if (!this.HeaderArrivedAndWasNotApplied(context.Request))
		{
			return;
		}

		// Two requests can get this far at once. Exchange hands back the previous value, so exactly one logs.
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

		// With the feature on, original-for is the signal, not forwarded-for: the framework rewrites the
		// forwarded header as it consumes entries and removes it once empty, so what is left says nothing about
		// whether an address was replaced. Original-for is written only when one was.
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
