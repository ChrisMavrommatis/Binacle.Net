using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.Middleware;

internal class RequestDebugMiddleware
{
	internal const string Path = "/_debug";

	private readonly RequestDelegate next;
	private readonly IHostEnvironment hostEnvironment;
	private readonly IOptions<FeatureOptions> featureOptions;
	private readonly TimeProvider timeProvider;

	public RequestDebugMiddleware(
		RequestDelegate next,
		IHostEnvironment hostEnvironment,
		IOptions<FeatureOptions> featureOptions,
		TimeProvider timeProvider
	)
	{
		this.next = next;
		this.hostEnvironment = hostEnvironment;
		this.featureOptions = featureOptions;
		this.timeProvider = timeProvider;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		if (!context.Request.Path.StartsWithSegments(Path))
		{
			await this.next(context);
			return;
		}

		var request = context.Request;
		var connection = context.Connection;
		var now = this.timeProvider.GetUtcNow();
		var startedAt = new DateTimeOffset(Process.GetCurrentProcess().StartTime.ToUniversalTime());

		var output = new StringBuilder();

		output.AppendLine("Binacle.Net debug. Echoes your own request, so treat the output as you would your own");
		output.AppendLine("credentials. Do not paste it anywhere public.");
		output.AppendLine();

		// The socket peer is the only address in here that a caller cannot choose. Everything under [headers]
		// is text somebody wrote, which is the whole point of showing both.
		output.AppendLine("[connection]");
		output.AppendLine($"RemoteAddr:   {connection.RemoteIpAddress}:{connection.RemotePort}");
		output.AppendLine($"LocalAddr:    {connection.LocalIpAddress}:{connection.LocalPort}");
		output.AppendLine($"ConnectionId: {connection.Id}");
		output.AppendLine($"Protocol:     {request.Protocol}");
		output.AppendLine($"IsHttps:      {request.IsHttps}");
		output.AppendLine();

		output.AppendLine("[request]");
		output.AppendLine($"{request.Method} {request.Path}{request.QueryString} {request.Protocol}");
		output.AppendLine($"Scheme: {request.Scheme}");
		output.AppendLine($"Host:   {request.Host}");
		output.AppendLine();

		// Repeated headers are listed once per value rather than joined. X-Forwarded-For can arrive as several
		// headers instead of one comma separated list, and which one it is changes how it should be read.
		output.AppendLine("[headers]");
		foreach (var header in request.Headers)
		{
			if (string.Equals(header.Key, "Cookie", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			foreach (var value in header.Value)
			{
				output.AppendLine($"{header.Key}: {value}");
			}
		}

		output.AppendLine();

		output.AppendLine("[cookies]");
		if (request.Cookies.Count == 0)
		{
			output.AppendLine("(none)");
		}
		else
		{
			foreach (var cookie in request.Cookies)
			{
				output.AppendLine($"{cookie.Key}: {cookie.Value}");
			}
		}

		output.AppendLine();

		output.AppendLine("[server]");
		output.AppendLine($"Version:     {Environment.GetEnvironmentVariable("BINACLE_VERSION") ?? "Unknown"}");
		output.AppendLine($"Environment: {this.hostEnvironment.EnvironmentName}");
		output.AppendLine($"Machine:     {Environment.MachineName}");
		output.AppendLine($"ProcessId:   {Environment.ProcessId}");
		output.AppendLine($"StartedAt:   {startedAt:O}");
		output.AppendLine($"Uptime:      {now - startedAt:d\\.hh\\:mm\\:ss}");
		output.AppendLine($"UtcNow:      {now:O}");
		output.AppendLine($"Features:    {string.Join(", ", this.featureOptions.Value.EnabledFeatures.Order())}");

		context.Response.StatusCode = StatusCodes.Status200OK;
		context.Response.ContentType = "text/plain; charset=utf-8";
		await context.Response.WriteAsync(output.ToString());
	}
}
