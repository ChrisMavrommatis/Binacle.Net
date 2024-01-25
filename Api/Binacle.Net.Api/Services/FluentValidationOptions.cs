using FluentValidation;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.Services;

public class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    private readonly IValidator<TOptions> validator;

    public string? Name { get; }
    public FluentValidationOptions(string? name, IValidator<TOptions> validator)
    {
        this.Name = name;
        this.validator = validator;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (this.Name != null && this.Name != name)
        {
            return ValidateOptionsResult.Skip;
        }

        ArgumentNullException.ThrowIfNull(options);

        var validationResult = this.validator.Validate(options);
        if (validationResult.IsValid)
            return ValidateOptionsResult.Success;

        var errors = validationResult.Errors.Select(x => $"Options validation failed of '{x.PropertyName}' with error: '{x.ErrorMessage}'.");
        return ValidateOptionsResult.Fail(errors);
    }
}
