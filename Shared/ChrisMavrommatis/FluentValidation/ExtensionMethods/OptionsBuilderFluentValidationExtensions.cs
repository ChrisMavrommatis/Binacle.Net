using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentValidation;

public static class OptionsBuilderFluentValidationExtensions
{
	public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
	{
		optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(sp =>
		{
			var validator = sp.GetRequiredService<IValidator<TOptions>>();
			return new ChrisMavrommatis.Services.FluentValidationOptions<TOptions>(optionsBuilder.Name, validator);
		});
		return optionsBuilder;
	}
}
