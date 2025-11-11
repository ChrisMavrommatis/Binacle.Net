using Binacle.Net.Kernel.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Configuration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CorsOptions: IConfigurationOptions
{
	public static string FilePath => "Cors.json";
	public static string SectionName => "Cors";
	public static bool Optional => true;
	public static bool ReloadOnChange => true;
	public static string? GetEnvironmentFilePath(string environment) => $"Cors.{environment}.json";

	
	public bool Enabled {get;set;}
	public CorsPolicyOptions? Frontend {get;set;}
}

public class CorsPolicyOptions
{
	public string[]? AllowedOrigins {get;set;}
}


internal class CorsOptionsOptionsValidator : AbstractValidator<CorsOptions>
{
	public CorsOptionsOptionsValidator()
	{
		RuleFor(x => x.Frontend)
			.NotNull()
			.When(x => x.Enabled)
			.WithMessage("Frontend Policy Option must be provided when CORS is enabled.");

		RuleFor(x => x.Frontend)
			.ChildRules(childRule =>
			{
				childRule.RuleFor(x => x!.AllowedOrigins)
					.NotNull()
					.NotEmpty()
					.WithMessage("AllowedOrigins must be provided and contain at least one origin.");
			})
			.When(x => x.Enabled);

	}
}
