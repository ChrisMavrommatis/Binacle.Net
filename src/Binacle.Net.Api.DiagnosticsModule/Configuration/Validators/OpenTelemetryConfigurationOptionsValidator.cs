using Binacle.Net.Api.DiagnosticsModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Api.DiagnosticsModule.Configuration.Validators;

internal class OpenTelemetryConfigurationOptionsValidator : AbstractValidator<OpenTelemetryConfigurationOptions>
{
	public OpenTelemetryConfigurationOptionsValidator()
	{
		// When(options => options.IsEnabled(), () =>
		// {
		// 	When(innerOptions => !string.IsNullOrEmpty(innerOptions.GlobalOtlpEndpoint), () =>
		// 	{
		// 		RuleFor(x => x.Metrics)
		// 			.ChildRules(x => x
		// 				.RuleFor(metrics => metrics.OtlpEndpoint)
		// 				.Empty()
		// 			);
		//
		// 		RuleFor(x => x.Tracing)
		// 			.ChildRules(x => x
		// 				.RuleFor(tracing => tracing.OtlpEndpoint)
		// 				.Empty()
		// 			);
		// 		
		// 		RuleFor(x => x.Logging)
		// 			.ChildRules(x => x
		// 				.RuleFor(logging => logging.OtlpEndpoint)
		// 				.Empty()
		// 			);
		//
		// 	}).Otherwise(() =>
		// 	{
		// 		RuleFor(x => x)
		// 			.Must(x => x.Metrics.IsEnabled() || x.Tracing.IsEnabled() || x.Logging.IsEnabled())
		// 			.WithMessage("At least one of the following properties must be set: 'GlobalOtlpEndpoint', 'Metrics.OtlpEndpoint', 'Tracing.OtlpEndpoint', 'Logging.OtlpEndpoint'");
		// 	});
		//
		// });
	}
}
