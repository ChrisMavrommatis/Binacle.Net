using Microsoft.ApplicationInsights.Channel;

namespace Binacle.Net.Api.ServiceIntegrationTests.MockServices;

public class FakeTelemetryChannel : ITelemetryChannel
{
	public bool IsFlushed { get; private set; }
	public bool? DeveloperMode { get; set; }
	public string EndpointAddress { get; set; }

	public void Send(ITelemetry item)
	{
	}

	public void Flush()
	{
		IsFlushed = true;
	}

	public void Dispose()
	{
	}
}
