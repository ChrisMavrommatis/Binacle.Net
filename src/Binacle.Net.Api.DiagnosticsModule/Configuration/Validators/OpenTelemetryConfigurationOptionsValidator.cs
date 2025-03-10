using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using FluentValidation;
using OpenTelemetry.Exporter;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Validators;

internal class OpenTelemetryConfigurationOptionsValidator : AbstractValidator<OpenTelemetryConfigurationOptions>
{
	public OpenTelemetryConfigurationOptionsValidator()
	{
		RuleFor(x => x.Otlp)
			.SetValidator(x => new OtlpExporterConfigurationsOptionsValidator());

		RuleFor(x => x.AzureMonitor)
			.SetValidator(x => new AzureMonitorConfigurationOptionsValidator());
		
		RuleFor(x => x.Metrics)
			.ChildRules(child =>
			{
				child.RuleFor(x => x.Otlp)
					.SetValidator(x => new OtlpExporterConfigurationsOptionsValidator());
			});
		
		RuleFor(x => x.Tracing)
			.ChildRules(child =>
			{
				child.RuleFor(x => x.Otlp)
					.SetValidator(x => new OtlpExporterConfigurationsOptionsValidator());
			});
		
		RuleFor(x => x.Logging)
			.ChildRules(child =>
			{
				child.RuleFor(x => x.Otlp)
					.SetValidator(x => new OtlpExporterConfigurationsOptionsValidator());
			});
	}
}
