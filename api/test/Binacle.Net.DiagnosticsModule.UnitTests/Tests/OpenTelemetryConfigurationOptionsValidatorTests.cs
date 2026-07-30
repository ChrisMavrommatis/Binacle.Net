using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Configuration.Validators;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// Telemetry is off in a default deployment, so the empty section has to stay valid. What must be caught is a
// value the exporters cannot use — those surface at startup, long before anyone looks at a dashboard.
[Trait("Behavioral Tests", "Ensures OpenTelemetry configuration is validated as expected")]
public class OpenTelemetryConfigurationOptionsValidatorTests
{
	private readonly OpenTelemetryConfigurationOptionsValidator validator = new();

	private static OpenTelemetryConfigurationOptions OptionsWith(string? protocol = null, float samplingRatio = 1.0f)
		=> new()
		{
			Otlp = new OtlpExporterConfigurationOptions { Protocol = protocol },
			AzureMonitor = new AzureMonitorConfigurationOptions { SamplingRatio = samplingRatio }
		};

	[Theory]
	[InlineData(null)] // unset falls back to grpc in GetOtlpExportProtocol
	[InlineData("")]
	[InlineData("grpc")]
	[InlineData("httpProtobuf")]
	public void A_supported_otlp_protocol_is_accepted(string? protocol)
	{
		this.validator.Validate(OptionsWith(protocol: protocol)).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("http")]
	[InlineData("HttpProtobuf")] // the check is case sensitive
	[InlineData("gRPC")]
	public void An_unsupported_otlp_protocol_fails_validation(string protocol)
	{
		this.validator.Validate(OptionsWith(protocol: protocol)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(0.1f)]
	[InlineData(0.5f)]
	[InlineData(1.0f)]
	public void A_sampling_ratio_inside_the_range_is_accepted(float samplingRatio)
	{
		this.validator.Validate(OptionsWith(samplingRatio: samplingRatio)).IsValid.ShouldBeTrue();
	}

	// 0 is the tempting value for "sample nothing" and it is rejected — turn the exporter off instead.
	[Theory]
	[InlineData(0f)]
	[InlineData(0.05f)]
	[InlineData(1.5f)]
	public void A_sampling_ratio_outside_the_range_fails_validation(float samplingRatio)
	{
		this.validator.Validate(OptionsWith(samplingRatio: samplingRatio)).IsValid.ShouldBeFalse();
	}
}
