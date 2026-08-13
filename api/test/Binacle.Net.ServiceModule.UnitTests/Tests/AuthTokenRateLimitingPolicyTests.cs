using System.Net;
using Binacle.Net.ServiceModule.Configuration;
using Binacle.Net.ServiceModule.Models;
using Binacle.Net.ServiceModule.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.UnitTests;

// The login throttle partitions on the connection address. It once read X-Forwarded-For directly, which let a
// caller reset their own limit by varying the header. These tests exist so that cannot come back.
[Trait("Behavioral Tests", "Ensures the auth token rate limit partitions on the connection address")]
public class AuthTokenRateLimitingPolicyTests
{
	private readonly AuthTokenRateLimitingPolicy policy;

	public AuthTokenRateLimitingPolicyTests()
	{
		var options = Options.Create(new RateLimiterConfigurationOptions
		{
			AuthToken = "FixedWindow::5/60",
			AuthTokenConfiguration = RateLimiterConfiguration.Parse("FixedWindow::5/60")
		});

		this.policy = new AuthTokenRateLimitingPolicy(options, NullLogger<AuthTokenRateLimitingPolicy>.Instance);
	}

	private static HttpContext ContextFor(string? remoteIpAddress, string? forwardedFor = null)
	{
		var context = new DefaultHttpContext();
		context.Connection.RemoteIpAddress = remoteIpAddress is null ? null : IPAddress.Parse(remoteIpAddress);
		if (forwardedFor is not null)
		{
			context.Request.Headers["X-Forwarded-For"] = forwardedFor;
		}

		return context;
	}

	[Fact]
	public void Two_Callers_Get_Two_Partitions()
	{
		var first = this.policy.GetPartition(ContextFor("192.168.1.5"));
		var second = this.policy.GetPartition(ContextFor("192.168.1.6"));

		first.PartitionKey.ShouldNotBe(second.PartitionKey);
	}

	[Fact]
	public void The_Same_Caller_Gets_One_Partition()
	{
		var first = this.policy.GetPartition(ContextFor("192.168.1.5"));
		var second = this.policy.GetPartition(ContextFor("192.168.1.5"));

		first.PartitionKey.ShouldBe(second.PartitionKey);
	}

	// The header is the caller's to write, so reaching the partition key would hand anyone a fresh limit on
	// every attempt.
	[Fact]
	public void A_Forged_Forwarded_Header_Does_Not_Move_The_Partition()
	{
		var withoutHeader = this.policy.GetPartition(ContextFor("192.168.1.5"));
		var withHeader = this.policy.GetPartition(ContextFor("192.168.1.5", forwardedFor: "10.9.9.9"));
		var withAnotherHeader = this.policy.GetPartition(ContextFor("192.168.1.5", forwardedFor: "10.9.9.10"));

		withHeader.PartitionKey.ShouldBe(withoutHeader.PartitionKey);
		withAnotherHeader.PartitionKey.ShouldBe(withoutHeader.PartitionKey);
	}

	// Everything without a resolvable address shares one bucket, which throttles them together rather than
	// letting them through unlimited.
	[Fact]
	public void An_Unknown_Caller_Falls_Into_A_Single_Named_Partition()
	{
		var partition = this.policy.GetPartition(ContextFor(null));

		partition.PartitionKey.ShouldBe("unknown");
	}
}
