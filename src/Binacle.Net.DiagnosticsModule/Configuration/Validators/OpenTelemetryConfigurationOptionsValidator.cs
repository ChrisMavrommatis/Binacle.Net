using Binacle.Net.DiagnosticsModule.Configuration.Models;
using FluentValidation;
using OpenTelemetry.Exporter;

namespace Binacle.Net.DiagnosticsModule.Configuration.Validators;

internal class OpenTelemetryConfigurationOptionsValidator : AbstractValidator<OpenTelemetryConfigurationOptions>
{
	public OpenTelemetryConfigurationOptionsValidator()
	{
		RuleFor(x => x.Otlp)
			.SetValidator(x => new OtlpExporterConfigurationsOptionsValidator());

		RuleFor(x => x.AzureMonitor)
			.SetValidator(x => new AzureMonitorConfigurationOptionsValidator());
	}
}

internal class OtlpExporterConfigurationsOptionsValidator : AbstractValidator<OtlpExporterConfigurationOptions>
{
	public OtlpExporterConfigurationsOptionsValidator()
	{
		When(x =>  !string.IsNullOrEmpty(x.Protocol), () =>
		{
			RuleFor(x => x.Protocol)
				.Must(x => x == "grpc" || x == "httpProtobuf")
				.WithMessage("The protocol must be either 'grpc' or 'httpProtobuf'");
		});
		
	}
	
}

internal class AzureMonitorConfigurationOptionsValidator : AbstractValidator<AzureMonitorConfigurationOptions?>
{
	public AzureMonitorConfigurationOptionsValidator()
	{
		When(x => x is not null, () =>
		{
			RuleFor(x => x!.SamplingRatio)
				.InclusiveBetween(0.1f, 1.0f);
		});
	}
}
