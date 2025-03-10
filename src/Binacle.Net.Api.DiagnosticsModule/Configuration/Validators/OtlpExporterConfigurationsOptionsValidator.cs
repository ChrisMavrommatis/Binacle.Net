using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Validators;

internal class OtlpExporterConfigurationsOptionsValidator : AbstractValidator<OtlpExporterConfigurationOptions?>
{
	public OtlpExporterConfigurationsOptionsValidator()
	{
		When(x => x is not null, () =>
		{
			RuleFor(x => x!.Protocol)
				.NotEmpty();
				
			RuleFor(x => x!.Protocol)
				.Must(x => x == "grpc" || x == "httpProtobuf")
				.WithMessage("The protocol must be either 'grpc' or 'httpProtobuf'");

			When(x => !string.IsNullOrEmpty(x!.Headers), () =>
			{
				// pattern 'key=value,key=value'
				RuleFor(x => x!.Headers)
					.Matches("^([a-zA-Z0-9-_]+=[^=,]+)(,[a-zA-Z0-9-_]+=[^=,]+)*$")
					.WithMessage("The headers must be in the format 'key1=value1,key2=value2'");
			});
		});
		
	}
	
}
