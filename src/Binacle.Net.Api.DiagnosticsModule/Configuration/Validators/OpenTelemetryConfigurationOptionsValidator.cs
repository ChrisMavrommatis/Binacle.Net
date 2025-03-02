using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Validators;

internal class OpenTelemetryConfigurationOptionsValidator : AbstractValidator<OpenTelemetryConfigurationOptions>
{
	public OpenTelemetryConfigurationOptionsValidator()
	{
		
	}	
}
